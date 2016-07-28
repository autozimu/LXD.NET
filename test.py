#!/usr/bin/env python3

import os.path
import requests

conf_dir = os.path.expanduser('~/repo/lxd.NET/lxd/cert')
crt = os.path.join(conf_dir, 'client.crt')
key = os.path.join(conf_dir, 'client.key')

print(requests.get('https://ubuntu:8443/1.0', verify=False, cert=(crt, key)).text)
