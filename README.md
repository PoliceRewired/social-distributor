# Police Rewired social

A tool for sharing key news with our community.

## Usage

### Automation

This tool is intended to run as an AWS lambda.

### Scripts

After you have set up your environment and AWS cli client credentials, you can use the following scripts for quick actions:

* `init.sh` - install the dotnet amazon lambda tools
* `build.sh` - rebuild the lambda project
* `test.sh` - run tests against the lambda project
* `deploy.sh` - deploy the lambda project to AWS
* `dry-run.sh` - ask the lambda to confirm it could post to all the networks (does not post)

### Manual testing

You can test the tool locally, and issue posts manually using the associated command line app `DistributeSocialApp`.

* Enter the directory: `cd Distributor/DistributeSocialApp`
* Pick a name for your environment, eg. `prod`
* Create an env file in the same directory as your binary, eg. `.env.prod`
* Place your environment variables in the `.env.prod` file, as `KEY=value`, one per line.
* Configure that file in your project: Build action: `Content`, Copy to output directory: `Always copy`
* Build and run the app:

```
dotnet run <environment> <network> "<text>"
```

For environment, provide `prod` if you named your environment file `.env.prod`

For network, provide one of: `twitter`, `facebook`, `linkedin`, `discord`

## The lambda

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

### Invoking the lambda

Invoke the lambda from your local machine with `dotnet lambda invoke-function`, provide the local profile and a payload.

```
dotnet lambda invoke-function DistributeSocialLambda --profile sa-social-distributor --payload '{ some: "json here" }'
```

### Test Post

You can use `dry-run.sh`, or provide the following JSON:

```json
{ "command": "dry-run", "networks": [ "facebook", "twitter", "discord" ], "message": "test message invocation" }
```

### Send Post

You can use `post-all.sh "Test message"`, or provide the following JSON:

```json
{ "command": "post", "networks": [ "facebook", "twitter", "discord" ], "message": "Test message." }
```

### Test Auto

Provide the following JSON:

```json
{ "command": "auto-dry-run", "networks": [ "facebook", "twitter", "discord" ], "postsCsvUrl": "", "rulesCsvUrl": "" }
```

### Auto Post

Provide the following JSON:

```json
{ "command": "auto", "networks": [ "facebook", "twitter", "discord" ], "postsCsvUrl": "", "rulesCsvUrl": "" }
```

## CSV formats

CSVs provided for auto mode show conform to the following format:

### Posts CSV

| ListId | Content |
|--------|---------|
| Identity of the list | Text to include in a post |

### Rules CSV

A list is selected randomly, and weighted by ratios:

| ListId | Ratio |
|--------|---------|
| Identity of the list | Integer ratio indicating how to balance posts from this list amongst all lists. |

## State variables

Auto mode tracks state using a JSON file stored in an S3 bucket.
Provide the following environment variables:

* `S3_STATE_BUCKET`
* `S3_STATE_KEY`

## Social network variables

Provide the following environment variables per network you intend to use.

### Twitter

Provide the following environment variables:

* `TWITTER_CONSUMER_KEY`
* `TWITTER_CONSUMER_KEY_SECRET`
* `TWITTER_ACCESS_TOKEN`
* `TWITTER_ACCESS_TOKEN_SECRET`

### Facebook

* See: https://ermir.net/topic-10/how-to-publish-a-message-to-a-facebook-page-using-a-dotnet-console-application

1. At the [Graph API Explorer](https://developers.facebook.com/tools/explorer), generate a **user token** for the right app, the right page, and with permissions: `pages_show_list`, `pages_read_engagement`, `pages_manage-posts`, `public_profile`
2. At the [Token Debugger](https://developers.facebook.com/tools/accesstoken/), examine that user token. (Expected lifetime is ~60 days.)
3. At the [Graph API Explorer](https://developers.facebook.com/tools/explorer/?method=GET&path=me%2Faccounts&version=v10.0), provide that access token, and explore `me/accounts`
4. The `data[0].access_token` field should contain an indefinite access token. Store it safely.
5. The `data[0].id` field should contain the page id. Store it safely.
5. At the [Token Debugger](https://developers.facebook.com/tools/accesstoken/), ensure that the token does not expire.

Provide the following environment variables:

* `FACEBOOK_PAGE_ID` - the id of the page to post to
* `FACEBOOK_ACCESS_TOKEN` - an unlimited access token derived from the user token

### Discord

Provide the following environment variables:

* `DISCORD_TOKEN` - access token belonging to the discord app
* `DISCORD_SERVER_ID` - id of the server (enable developer view for a quicker, easier view of this)
* `DISCORD_CHANNEL` - the name of the channel you wish to post to (the app will look it up)

### LinkedIn

* See: https://www.linkedin.com/developers/
* See: https://docs.microsoft.com/en-gb/linkedin/consumer/integrations/self-serve/share-on-linkedin
