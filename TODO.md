- Make each domain object has a reference to the API that created it. This
  way, no need to make API as thread static (which is obviously not safe).
- Docs.
- Tests.
