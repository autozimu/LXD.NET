# LXD.NET
[![Build status](https://ci.appveyor.com/api/projects/status/d9hk73a1opdlhxp9?svg=true)](https://ci.appveyor.com/project/JunfengLi/lxd-net)

LXD client implemented in C#.

# Example

```CSharp
using LXD;

Client client = new Client(
    apiEndpoint: "https://your-lxd-service:8443",
    clientCertificateFilename: "your-client-certificate.p12",
    password: "your-client-certificate-password");

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
