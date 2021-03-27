# AWS Lambdas

Notes on developing, deploying AWS lambdas, and the configuration of the AWS environment.

## Documentation

* C# lambdas: https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-cli.html
* AWS CLI: https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2-mac.html
* AWS Config: https://docs.aws.amazon.com/cli/latest/userguide/cli-configure-quickstart.html

### Set up AWS CLI

Create a service account IAM user. In our case: `sa-social-distributor`

You'll need to safely store the access key id and secret access key.

Configure a local profile to match, and provide the access key id, and secret access key.

```
aws configure --profile sa-social-distributor
```

### Template

To create a template lambda project:

```
dotnet new -i Amazon.Lambda.Templates
dotnet new lambda.EmptyFunction --help
dotnet new lambda.EmptyFunction --name DistributeSocialLambda
```

### Deploy

The Amazon lambda tools are required (installed by `init.sh`):

```
dotnet tool install -g Amazon.Lambda.Tools
dotnet tool update -g Amazon.Lambda.Tools
```

Use dotnet lambda to deploy the function naming the profile and role:

```
dotnet lambda deploy-function --profile sa-social-distributor DistributeSocialLambda --function-role role-social-distributor
```

If the `role-social-distributor` role doesn't exist, you'll need to create it first. You'll also have to attach an IAM Policy to the role. `AWSLambdaExecute`, looks about right.

#### Deployment environment

You can provide the environment variables to the lambda through AWS web interface.

If you'd rather do it from the command line, you can use the `--environment-variables` option to provide the various secrets, as: `<key1>=<value1>;<key2>=<value2>` etc.

You could also add an `environment-variables` key in the `aws-lambda-tools-default.json` file, but be careful not to include your secrets in a public github repo.
