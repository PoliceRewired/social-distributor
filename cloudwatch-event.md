# CloudWatch Events

A cloudwatch event is configured with the following AWS crontab rule:

```
30 8-19/2 ? * * *
```

* It will trigger the `DistributeSocialLambda` function at `08:30`, `10:30`, `12:30`, `14:30`, `16:30`, `18:30` each day.
* See: [schedule expressions for rules](https://docs.aws.amazon.com/AmazonCloudWatch/latest/events/ScheduledEvents.html)

The lambda is provided the following input:

```json
{ "command": "auto-message", "networks": ["facebook", "twitter", "discord"] }
```
