#!/bin/zsh

PAYLOAD=$(cat << 'EOF'
{ 
    "command": "post-message", 
    "networks": [ "facebook", "twitter" ], 
    "text": "Standard Operating Procedure for the police-issue MetWhistle, 2012",
    "tags": "#sop #whistle #policing #crime #disorder",
    "linkUrl": "https://github.com/PoliceRewired/social-distributor-resources/raw/main/docs/metwhistle-sop.pdf",
    "imageUrl": "https://github.com/PoliceRewired/social-distributor-resources/raw/main/docs/metwhistle-cover.png"
}
EOF
)

echo $PAYLOAD

dotnet lambda invoke-function DistributeSocialLambda \
  --region eu-west-2 \
  --profile sa-social-distributor \
  --payload $PAYLOAD
