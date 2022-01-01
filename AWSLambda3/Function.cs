using System;
using Amazon.Lambda.Core;
using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using static Gremlin.Net.Process.Traversal.AnonymousTraversalSource;
using static Gremlin.Net.Process.Traversal.__;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace helloNeptune
{
    
    public class Function
    {

        /// <summary>
        /// Populates and queries a Neptune graph database.
        /// </summary>
        /// <param name="input">one of the following: setup (init data), people (list all), direct (list direct reports), subs (list subordinates)</param>
        /// <param name="context">function context object used for logging</param>
        /// <returns></returns>
        public string FunctionHandler(string input, ILambdaContext context)
        {
            var endpoint = "[database-helloneptune.cluster-url].neptune.amazonaws.com";

            try
            {
                var gremlinServer = new GremlinServer(endpoint, 8182, enableSsl: true);
                var gremlinClient = new GremlinClient(gremlinServer);
                var remoteConnection = new DriverRemoteConnection(gremlinClient, "g");
                var g = Traversal().WithRemote(remoteConnection);

                switch (input)
                {
                    case "setup":
                        context.Logger.LogLine("Dropping all edges");
                        g.E().Drop().Iterate();
                        context.Logger.LogLine("Dropping all vertices");
                        g.V().Drop().Iterate();

                        context.Logger.LogLine("Adding vertices");

                        var alice = g.AddV("person").Property("name", "Alice").Property("role", "Manager").Next();
                        var bob = g.AddV("person").Property("name", "Bob").Property("role", "Engineer").Next();
                        var justin = g.AddV("person").Property("name", "Justin").Property("role", "Writer").Next();
                        var ashok = g.AddV("person").Property("name", "Ashok").Property("role", "Intern").Next();
                        var jamal = g.AddV("person").Property("name", "Jamal").Property("role", "Intern").Next();

                        context.Logger.LogLine("Adding edges");

                        g.V(alice).AddE("manages").To(bob).Iterate();
                        g.V(alice).AddE("manages").To(justin).Property("weight", 0.5).Iterate();
                        g.V(justin).AddE("manages").To(ashok).Property("weight", 0.5).Iterate();
                        g.V(justin).AddE("manages").To(jamal).Property("weight", 0.5).Iterate();
                        break;
                    case "people":
                        {
                            context.Logger.LogLine("Listing all people");
                            var people = g.V()
                             .HasLabel("person")
                             .Project<object>("Name", "Role")
                                 .By("name")
                                 .By("role")
                             .ToList();

                            context.Logger.LogLine($"Name     Role");
                            foreach (var person in people)
                            {
                                context.Logger.LogLine($"{person["Name"],-8} {person["Role"],-8}");
                            }
                        }
                        break;
                    case "directs":
                        {
                            context.Logger.LogLine("Listing Alice's direct reports");
                            var people = g.V().Has("name", "Alice")
                                .Out("manages")
                             .Project<object>("Name", "Role")
                                 .By("name")
                                 .By("role")
                             .ToList();

                            context.Logger.LogLine($"Name     Role");
                            foreach (var person in people)
                            {
                                context.Logger.LogLine($"{person["Name"],-8} {person["Role"],-8}");
                            }
                        }
                        break;
                    case "subs":
                        {
                            context.Logger.LogLine("Listing Alice's subordinates");

                            var people = g.V().Has("name", "Alice")
                                .Repeat(Out("manages")).Times(2).Emit()
                                .Project<object>("Name", "Role")
                                .By("name")
                                .By("role")
                            .ToList();

                            context.Logger.LogLine($"Name     Role");
                            foreach (var person in people)
                            {
                                context.Logger.LogLine($"{person["Name"],-8} {person["Role"],-8}");
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException($"Unrecognized function input: {input} - try setup, people, directs, subs");
                }
                context.Logger.LogLine("Successful");
                return "success";
            }
            catch (Exception e)
            {
                context.Logger.LogLine(e.ToString());
                return $"exception: {e}";
            }
        }
    }
}
