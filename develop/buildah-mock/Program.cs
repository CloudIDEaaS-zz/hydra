using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace buildah
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var parseResult = CommandLineParser.ParseArgs<ParseResult>(args, (result, arg) =>
            {
                result.NonswitchArgs.Add(arg);
            },
            (result, _switch, switchArg) =>
            {
                switch (_switch)
                {
                    case SwitchCommands.TAG:
                        result.Tag = switchArg;
                        break;
                    case SwitchCommands.CREDS:
                        result.Creds = switchArg;
                        break;
                }
            });

            switch (parseResult.PrimaryCommand)
            {
                case "bud":
                    {
                        var output = @"STEP 1: FROM mcr.microsoft.com / dotnet / aspnet:3.1 AS base
STEP 2: WORKDIR / app
STEP 3: EXPOSE 80
STEP 4: EXPOSE 443
STEP 5: RUN useradd - u 8877 serviceaccount
STEP 6: USER serviceaccount
-- > 15c96f2dedf
STEP 7: FROM mcr.microsoft.com / dotnet / sdk:3.1 AS build
STEP 8: WORKDIR / src
STEP 9: COPY ""MinimalAngularWebProject.csproj"".
STEP 10: RUN dotnet restore ""./MinimalAngularWebProject.csproj""
  Determining projects to restore...
  Restored / src / MinimalAngularWebProject.csproj(in 6.28 sec).
STEP 11: FROM 15c96f2dedf508039dfcff45a0c8bef1a2f0389e08da03a9855b65552576aee3 AS final
STEP 12: WORKDIR / app
STEP 13: COPY. .
STEP 14: ENTRYPOINT[""dotnet"", ""MinimalAngularWebProject.dll""]
STEP 15: COMMIT minimalangularwebproject.web:1.0.0.51
--> c6ce7cec400
c6ce7cec400e93d2dea014ff08444f8e6623fc30b894746d1e6e35da5d45b203";

                        foreach (var line in output.GetLines())
                        {
                            Console.WriteLine(line);
                            Thread.Sleep(NumberExtensions.GetRandomIntWithinRange(100, 1000));
                        }
                    }

                    break;

                case "push":

                    {
                        var output = @"Getting image source signatures
Copying blob 971c233636bd done
Copying blob 2d4f6a76e010 done
Copying blob dc04796a8d80 done
Copying blob 98312369ae3d done
Copying blob 02c055ef67f5 done
Copying blob 4e0c8e5862b8 done
Copying blob 24e24724a5b4 done
Copying config c6ce7cec40 done
Writing manifest to image destination
Storing signatures";

                        foreach (var line in output.GetLines())
                        {
                            Console.WriteLine(line);
                            Thread.Sleep(NumberExtensions.GetRandomIntWithinRange(100, 1000));
                        }
                    }

                    break;

            }
        }
    }
}
