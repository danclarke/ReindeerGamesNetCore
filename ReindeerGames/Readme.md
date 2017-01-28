# Reindeer Games - AWS Lambda Function

This code is a C# port of the Reindeer sample from: https://github.com/amzn/alexa-skills-kit-json. If trying out the sample, I found it best to add the following text answers to the answer list:

* One
* Two
* Three
* Four

The code is tied specifically to AWS Lambda functions, and will not work outside of this environment.

## Performance

Thanks to some amazing work by the MS engineers on .NET Core, execution time is much faster than with the original NodeJS sample after compilation. RAM usage is a little higher than NodeJS, but not drastically so.

## AWS Toolkit

You'll need the AWS toolkit to deploy this code: https://aws.amazon.com/visualstudio/
