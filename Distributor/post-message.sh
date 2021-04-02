#!/bin/zsh

PAYLOAD=$(cat << 'EOF'
{ 
    "command": "post-message", 
    "networks": [ "facebook", "twitter", "discord", "reddit" ], 
    "text": "This is a test of our social sharing tool.",
    "tags": "#test",
    "linkUrl": "https://policerewired.org",
    "imageUrl": "https://github.com/PoliceRewired/social-distributor-resources/raw/main/images/test-card.jpeg"
}
EOF
)

echo $PAYLOAD

dotnet lambda invoke-function DistributeSocialLambda \
  --region eu-west-2 \
  --profile sa-social-distributor \
  --payload $PAYLOAD
