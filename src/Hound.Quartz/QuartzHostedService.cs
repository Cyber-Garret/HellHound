using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Hound.Quartz;

internal class QuartzHostedService : IHostedService
{
	private IScheduler? Scheduler { get; set; }

	private readonly IOptions<QuartzHostedServiceOptions> _options;
	private readonly ISchedulerFactory _schedulerFactory;
	private readonly IJobFactory _jobFactory;
	private readonly IEnumerable<JobSchedule> _jobSchedules;

	public QuartzHostedService(ISchedulerFactory schedulerFactory, IJobFactory jobFactory,
		IEnumerable<JobSchedule> jobSchedules, IOptions<QuartzHostedServiceOptions> options)
	{
		_schedulerFactory = schedulerFactory;
		_jobFactory = jobFactory;
		_jobSchedules = jobSchedules;
		_options = options;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
		Scheduler.JobFactory = _jobFactory;

		foreach (var jobSchedule in _jobSchedules)
		{
			var job = CreateJob(jobSchedule);
			var trigger = CreateTrigger(jobSchedule);

			await Scheduler.ScheduleJob(job, trigger, cancellationToken);
		}

		await Scheduler.Start(cancellationToken);
	}

	public async Task StopAsync(CancellationToken cancellationToken)
	{
		if (Scheduler != null)
			await Scheduler.Shutdown(_options.Value.WaitForJobsToComplete, cancellationToken);
	}

	/// <summary>
	/// Help auto create <see cref="IJobDetail"/> from <see cref="JobSchedule"/>
	/// </summary>
	private static IJobDetail CreateJob(JobSchedule schedule) =>
		JobBuilder
			.Create(schedule.JobType)
			.WithIdentity(schedule.JobType.FullName!)
			.WithDescription(schedule.JobType.Name)
			.Build();

	/// <summary>
	/// Help auto create <see cref="ITrigger"/> from <see cref="JobSchedule"/>
	/// </summary>
	private static ITrigger CreateTrigger(JobSchedule schedule) =>
		TriggerBuilder
			.Create()
			.WithIdentity($"{schedule.JobType.FullName}.trigger")
			.WithCronSchedule(schedule.CronExpression)
			.WithDescription(schedule.CronExpression)
			.Build();
}