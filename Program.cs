﻿namespace AliDDNS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string help_str = "Usage: ddns <list | update> -id <AccessKey ID> -secret <AccessKey Secret> -domain <domain> [-subdomain <subdomain>] [-record_id <record_id>] [-mode <local|net>]";
            int argc = args.Length;
            if (argc < 7)
            {
                Console.WriteLine(help_str);
                return;
            }

            string id = string.Empty;
            string secret = string.Empty;
            string domain = string.Empty;
            string subDomain = string.Empty;
            string? recordId = null;
            string ipMode = "local";  // 修改默认值为 local

            string command = args[0];
            if (command != "list" && command != "update")
            {
                Console.WriteLine(help_str);
                return;
            }

            try
            {
                for (int i = 1; i < argc; i++)
                {
                    switch (args[i])
                    {
                        case "-id":
                            id = args[++i];
                            break;
                        case "-secret":
                            secret = args[++i];
                            break;
                        case "-domain":
                            domain = args[++i];
                            break;
                        case "-subdomain":
                            subDomain = args[++i].ToLower();
                            break;
                        case "-record_id":
                            recordId = args[++i];
                            break;
                        case "-mode":
                            string mode = args[++i].ToLower();
                            if (mode != "local" && mode != "net")
                            {
                                Console.WriteLine("Invalid mode. Use 'local' or 'net'.");
                                return;
                            }
                            ipMode = mode;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine(help_str);
                return;
            }

            if (!string.IsNullOrEmpty(subDomain) && !string.IsNullOrEmpty(recordId))
            {
                Console.WriteLine("[WARN] Subdomain would be ignored if record_id is specified.");
            }

            AccessKey accessKey = new(id, secret);
            ServiceConfiguration config = new(domain, accessKey, subDomain, recordId, ipMode);

            var ddns = new AliyunDDNSUpdater(config);

            if (command == "list")
            {
                ListRecords(ddns);
            }
            else if (command == "update")
            {
                UpdateDNS(ddns);
            }
        }

        private static void ListRecords(AliyunDDNSUpdater ddns)
        {
            string records = ddns.ListRecords();
            Console.WriteLine(records);
        }

        private static void UpdateDNS(AliyunDDNSUpdater ddns)
        {

            bool result = ddns.UpdateDNS();
            if (result)
            {
                Console.WriteLine("Updated.");
            }
            else
            {
                Console.WriteLine("Failed update.");
            }
        }
    }
}