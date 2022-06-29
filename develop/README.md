# MailSlot
A C# implementation of the Windows Mailslot API

## Usage
The mailslot api is a Single Consumer Multiple Sender IPC system
natively supported by windows. A single server can listen to a
mail slot and any number of clients. Below is a naive example
that counts to 10, printing the numbers to the terminal.

```c#
using System;
using MailSlot;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new MailslotServer("round_trip"))
            {
                using (var client = new MailslotClient("round_trip"))
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var msg = $"Message {i}";
                        client.SendMessage(msg);
                        var read = server.GetNextMessage();
                        Console.WriteLine($"> {read}");
                    }
                }
            }
        }
    }
}
```