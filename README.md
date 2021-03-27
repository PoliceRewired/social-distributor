# Police Rewired social

A tool for sharing key news with our community.

## Usage

This tool is intended to run as an AWS lambda.

### Scripts

After you have set up your environment and AWS cli client credentials, you can use the following scripts for quick actions:

* `init.sh` - install the dotnet amazon lambda tools
* `build.sh` - rebuild the lambda project
* `deploy.sh` - deploy the lambda project to AWS
* `post-dry-run.sh` - composes a post from the input parameters, does not send to any social networks
* `post-message.sh` - composes a post from the input parameters, sends to the specified social networks
* `auto-dry-run.sh` - chooses a list, and a post from that list's plan, does not send to any social networks
* `auto-message.sh` - chooses a list, and a post from that list's plan, sends to the specified social networks
* `recalculate-plans.sh` - recalculates the plan for each list
* `clear-plans.sh` - clears all plans for all lists

### Invoking the lambda

Invoke the lambda from your local machine with `dotnet lambda invoke-function`, provide the local profile and a payload.

```
dotnet lambda invoke-function DistributeSocialLambda --profile sa-social-distributor --payload '{ some: "json here" }'
```

#### Sample inputs

```json
{ 
    "command": "post-dry-run", 
    "networks": [ "facebook", "twitter", "discord" ], 
    "text": "test message invocation",
    "tags": "#test",
    "linkUrl": "https://policerewired.org",
    "imageUrl": "https://policerewired.github.io/social-distributor-resources/images/test-card.jpeg"
}
```

```json
{ 
    "command": "auto-dry-run", 
    "networks": [ "facebook", "twitter", "discord" ], 
    "postsCsvUrl": "https://docs.google.com/spreadsheets/d/e/2PACX-1vSTur9fXXlA0eosd3jpzeOOe9Pi7Dk3T2LdKTobKjIrs2zZWZSMpwlNwDHHYl34wz1F_P2s7p7ya_3B/pub?gid=0&single=true&output=csv",
    "rulesCsvUrl": "https://docs.google.com/spreadsheets/d/e/2PACX-1vSTur9fXXlA0eosd3jpzeOOe9Pi7Dk3T2LdKTobKjIrs2zZWZSMpwlNwDHHYl34wz1F_P2s7p7ya_3B/pub?gid=1551543415&single=true&output=csv"
}
```

NB. Police Rewired library posts are published as CSV for consumption by the tool, and as an open data source:

* [posts](https://docs.google.com/spreadsheets/d/e/2PACX-1vSTur9fXXlA0eosd3jpzeOOe9Pi7Dk3T2LdKTobKjIrs2zZWZSMpwlNwDHHYl34wz1F_P2s7p7ya_3B/pub?gid=0&single=true&output=csv)
* [rules](https://docs.google.com/spreadsheets/d/e/2PACX-1vSTur9fXXlA0eosd3jpzeOOe9Pi7Dk3T2LdKTobKjIrs2zZWZSMpwlNwDHHYl34wz1F_P2s7p7ya_3B/pub?gid=1551543415&single=true&output=csv)

## CSV format

CSVs provided for auto mode should conform to the following formats.

### posts

| Field | Description |
|--------|---------|
| ListId | Identity of the list this post belongs to. |
| Text | Text of the post. |
| Tags | Any tags (include the `#`, eg. `#test` |
| URL | URL of the link this post is about. |
| ImageURL | URL to an image. (Optional.) |

Many of the images used by Police Rewired are held in the [social-distributor-resources](https://github.com/PoliceRewired/social-distributor-resources) repository.

### rules

For each post automatically chosen, a list is selected randomly, and weighted by ratio.

| Field | Description |
|--------|---------|
| ListId | Identity of the list. |
| Ratio  | Weight to attribute to the list. |
| Tags   | Default tags to apply to posts for this list (optional). |

## Manual testing

You can test the tool locally, and issue posts manually using the associated command line app `DistributeSocialApp`.

* Enter the directory: `cd Distributor/DistributeSocialApp`
* Pick a name for your environment, eg. `prod`
* Create an env file in the same directory as your binary, eg. `.env.prod`
* Place your environment variables in the `.env.prod` file, as `KEY=value`, one per line.
* Configure that file in your project: Build action: `Content`, Copy to output directory: `Always copy`
* Build and run the app:

```
dotnet run <environment> <network> <text> <tags> <link url> [<image url>]
```

* For environment, provide `prod` if you named your environment file `.env.prod`
* Surround values in `'single quotes'` to preserve spacing and special characters.
* For network, provide one of: `twitter`, `facebook`, `discord`

## State variables

When operating in automatic mode, the distributor keeps state using a JSON file stored in an S3 bucket.
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
