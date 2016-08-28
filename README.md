# LXD.NET

LXD client implemented in C#.

# Example

```CSharp
using LXD;

Client client = new Client(
    apiEndpoint: "https://lxd:8443",
    clientCertificateFilename: "client.p12",
    password: "");

Console.WriteLine(client.Trusted); // true

foreach (Domain.Container container in client.Containers) {
    Console.WriteLine(container.Name);
}
// alpline
// ubuntu

Domain.Container alpine = client.Containers.First();
foreach (string str in alpine.Exec(new[] {"cat", "/etc/issue"})) {
    Console.WriteLine(st);
}
// Welcome to Alpine Linux 3.4
// Kernel \r on an \m (\l)
```
