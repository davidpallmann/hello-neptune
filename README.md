# Hello, Neptune!

This is the code project for the [Hello, Neptune!](https://davidpallmann.hashnode.dev/hello-neptune) blog post. 

This episode: Amazon Neptune. In this Hello, Cloud blog series, we're covering the basics of AWS cloud services for newcomers who are .NET developers. If you love C# but are new to AWS, or to this particular service, this should give you a jumpstart.

In this post we'll introduce Amazon Neptune, a graph database service, and use it in a "Hello, Cloud" .NET program. We'll do this step-by-step, making no assumptions other than familiarity with C# and Visual Studio. We're using Visual Studio 2022 and .NET 6.

## Our Hello, Neptune Project

We'll use Visual Studio to create a simple "Hello, Lambda" function, deploy it to AWS, and test it. Our Lambda function will take a phone number input parameter (a string of digits) and return all the letter combinations.

Our Hello, Neptune project first creates a Neptune graph database in the AWS console and samples its Jupyter Notebook. We then write a .NET program that populates and queries data. Neptune only permits access from the same AWS VPC, so we'll be hosting today's .NET code in an AWS Lambda function.

See the blog post for the tutorial to create this project and run it on AWS.

