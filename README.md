# Police Rewired social distributor

A tool for sharing key news with our community.

## Usage

### Automation

This tool is intended to run as a lambda or recurring task in another environment.

### Manual use

You can test it, or issue posts manually using the associated command line app `DistributeSocialApp`.

* Enter the directory: `cd DistributeSocialApp`
* Create a file in the same directory as your binary called `.env.prod`
* Place your environment variables in an env file, as `KEY=value`, one per line.
* Save it as `.env.something` (ie. to name your environment), eg. `.env.prod`
* Build and run the app:

```
dotnet run <environment> <network> "<text>"
```

For environment, provide `prod` if you named your environment file `.env.prod`

For network, provide one of: `twitter`, `facebook`, `linkedin`, `discord`

## Environment variables

Provide the following environment variables per network you intend to use.

### Twitter

* `TWITTER_CONSUMER_KEY`
* `TWITTER_CONSUMER_KEY_SECRET`
* `TWITTER_ACCESS_TOKEN`
* `TWITTER_ACCESS_TOKEN_SECRET`

### Facebook

See: https://ermir.net/topic-10/how-to-publish-a-message-to-a-facebook-page-using-a-dotnet-console-application

* `FACEBOOK_PAGE_ID`
* `FACEBOOK_ACCESS_TOKEN`

### LinkedIn

TBC

### Discord

TBC
