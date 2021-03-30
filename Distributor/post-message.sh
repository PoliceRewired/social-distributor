#!/bin/zsh

PAYLOAD=$(cat << 'EOF'
{ 
    "command": "post-message", 
    "networks": [ "facebook", "twitter", "reddit", "discord" ], 
    "text": "This is a test of our social library, across all channels.",
    "tags": "#test",
    "linkUrl": "https://policerewired.org",
    "imageUrl": "https://policerewired.github.io/social-distributor-resources/images/test-card.jpeg"
}
EOF
)

echo $PAYLOAD

dotnet lambda invoke-function DistributeSocialLambda \
  --region eu-west-2 \
  --profile sa-social-distributor \
  --payload $PAYLOAD
