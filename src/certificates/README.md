# Certificates

The grpc service serves up the server certificate from the computer personal certificate store and will accept the client certificate. 
The grpc client sends over the client certificate from the computer personal certificate store. The server certificate is self-signed 
so it needs to be installed as a trusted certificate on the client machine for the connection to work.

Passwords for all certificates is `1111`. 

## Generate certificates

```shell
openssl req -x509 -newkey rsa:2048 -sha256 -keyout server-private.key -out server-public.crt -days 1200 -subj /CN=localhost -addext "subjectAltName=DNS:localhost"
openssl req -x509 -newkey rsa:2048 -sha256 -keyout client-private.key -out client-public.crt -days 1200 -subj /CN=grpc-client
openssl pkcs12 -export -name grpc-server -out server.pfx -inkey server-private.key -in server-public.crt
openssl pkcs12 -export -name grpc-client -out client.pfx -inkey client-private.key -in client-public.crt
```

The `-subj` for the server certificate should match the machine name or FQDN of the server the grpc service is running on. 
The `subjectAltName` can include alternative DNS mappings.

## Install certificates

1. Install server certificate as a computer trusted certificate on the grpc client machine.
2. Install client certificate as a computer personal certificate on the grpc cleint machine.
3. Install server certificate as a computer personal certificate on the grpc server machine.
4. Install client certificate as a computer personal certificate on the grpce server machine.
