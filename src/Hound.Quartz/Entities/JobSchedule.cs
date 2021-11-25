namespace Hound.Quartz.Entities;

internal class JobSchedule
{
	public JobSchedule(Type jobType, string cronExpression)
	{
		JobType = jobType;
		CronExpression = cronExpression;
	}

	internal Type JobType { get; }
	internal string CronExpression { get; }
}