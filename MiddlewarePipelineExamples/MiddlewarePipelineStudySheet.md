# ASP.NET Core Middleware Interview Prep (General Q&A)

Use this as interview practice. Try answering first, then compare.

## Core Questions

1. What is middleware in ASP.NET Core?
Answer: Middleware is a component in the HTTP pipeline that can inspect/modify the request, call the next component, and inspect/modify the response.

2. What is the middleware pipeline?
Answer: It is an ordered chain of middleware delegates executed for each request.

3. What is `RequestDelegate`?
Answer: A delegate type: `Task RequestDelegate(HttpContext context)`. It represents the next step in request handling.

4. Why is middleware order important?
Answer: Because each middleware wraps the next one. Wrong order can break auth, routing, caching, errors, or static files.

5. What does `await next(context)` do?
Answer: It passes control to the next middleware in the chain.

6. What is short-circuiting?
Answer: A middleware ends the pipeline by not calling `next`, usually after writing a response.

7. What is the difference between request and response flow?
Answer: Request flows top-down to `next`; response flows back up after `next` returns.

8. Where does `HttpContext` come from?
Answer: ASP.NET Core creates it per request and passes it through the pipeline.

9. Can middleware run code after `next`?
Answer: Yes. That is common for response headers, logging duration, cleanup, and metrics.

10. What happens if middleware throws an exception?
Answer: Exception middleware earlier in pipeline can catch it; if response already started, handling options are limited.

11. What is terminal middleware?
Answer: Middleware that handles response and does not call `next`.

12. Is middleware singleton or per request?
Answer: Conventional middleware instances are typically created once, but `Invoke/InvokeAsync` runs per request.

13. What is the method signature for conventional middleware?
Answer: Class constructor takes `RequestDelegate next`; request handler is `Task InvokeAsync(HttpContext context)`.

14. What is inline middleware?
Answer: Middleware defined with `app.Use(async (context, next) => { ... })`.

15. Difference between `Use`, `Run`, and `Map`?
Answer: `Use` can continue pipeline, `Run` is terminal, `Map` branches by path.

## Branching Questions

16. What does `Map("/x", ...)` do?
Answer: Creates a path branch; matching requests enter that branch pipeline.

17. Difference between `MapWhen` and `UseWhen`?
Answer: Both use predicates; `UseWhen` can rejoin main pipeline, `MapWhen` typically branches away.

18. When do you use `MapWhen`?
Answer: When branching by custom condition (headers, host, path pattern, etc.) and separate flow is needed.

19. When do you use `UseWhen`?
Answer: When conditionally adding middleware but still wanting normal main pipeline continuation.

20. What is branch rejoin?
Answer: After conditional branch middleware runs and calls `next`, control returns to main pipeline.

21. Does branch configuration run per request?
Answer: No. Branch configuration runs at startup; branch middleware execution runs per request.

22. Why is branch callback usually `Action<IApplicationBuilder>`?
Answer: Because it configures middleware pipeline and returns `void`.

23. Why can `Map` overload selection be confusing?
Answer: `Map` has endpoint and branch overloads; method signatures can accidentally match the wrong one.

24. How do you force branch overload clearly?
Answer: Use explicit `IApplicationBuilder` parameter in lambda.

25. Can you branch on query string directly with `Map`?
Answer: Not with path-only `Map`; use `MapWhen` predicate for query/headers/custom logic.

## Built-in Middleware Order

26. Typical order in web apps?
Answer: Exception handling, HTTPS redirection, static files, routing, authn, authz, endpoints.

27. Why must authentication run before authorization?
Answer: Authorization depends on authenticated user identity.

28. Where should `UseRouting` be?
Answer: Before auth middleware and before endpoint execution.

29. Where should `UseEndpoints`/mapped endpoints be?
Answer: Near end, after auth middleware and other request filters.

30. Why place `UseStaticFiles` early?
Answer: So static file requests can be served quickly without unnecessary middleware.

31. Why should exception middleware be early?
Answer: To catch exceptions from downstream middleware/components.

32. What if `UseAuthorization` is before `UseAuthentication`?
Answer: Authorization may run without a populated user and deny requests incorrectly.

33. Can order affect CORS?
Answer: Yes. CORS middleware placement relative to routing/endpoints matters.

34. Can multiple `Use` calls share state?
Answer: Yes, through `HttpContext.Items` or services; avoid unsafe shared mutable state.

35. Does `app.Run()` add middleware?
Answer: `Run` adds terminal behavior and starts host; middleware registration should happen before running.

## DI and Lifetime Questions

36. Can middleware constructor inject scoped services?
Answer: Not in conventional singleton-like middleware constructor. Inject scoped services into `InvokeAsync` parameters instead.

37. Why avoid scoped in middleware constructor?
Answer: Constructor runs once; scoped lifetime is per request, causing lifetime mismatch.

38. How does `IMiddleware` differ?
Answer: `IMiddleware` is resolved per request by DI, so constructor injection can use scoped services safely.

39. When to use `IMiddleware`?
Answer: When middleware needs scoped dependencies in constructor or cleaner DI integration.

40. What is transient middleware behavior?
Answer: Depends on registration and activation style; with `IMiddleware`, DI controls lifetime.

41. What is `IServiceProvider` role in middleware?
Answer: It resolves dependencies, typically via request services (`context.RequestServices`) per request scope.

42. Singleton vs Scoped vs Transient quick rule?
Answer: Singleton app-wide, Scoped per request, Transient per resolution.

43. How to access services inside inline middleware?
Answer: Resolve from `context.RequestServices` or close over resolved singleton services carefully.

## Error Handling and Security

44. Difference between Developer Exception Page and Exception Handler middleware?
Answer: Developer page is rich debug output for development; exception handler is production-safe error pipeline.

45. Why "response has already started" error appears?
Answer: Downstream already wrote headers/body; error middleware cannot fully replace response.

46. How to avoid leaking exception details?
Answer: Use production exception handler and return safe error messages.

47. Where to put rate limiting middleware?
Answer: Early enough to reject abusive traffic before expensive processing.

48. Where to put security headers middleware?
Answer: Typically early, but ensure it runs for intended responses.

49. Can middleware read request body multiple times?
Answer: Not by default; enable buffering when needed and reset stream position.

50. Why be careful with body buffering?
Answer: It affects memory/performance, especially for large payloads.

## Performance and Reliability

51. Middleware performance tips?
Answer: Keep logic small, avoid blocking I/O, short-circuit early when possible, and avoid unnecessary allocations.

52. Why avoid sync-over-async in middleware?
Answer: It can block thread pool threads and hurt throughput.

53. How to measure middleware latency?
Answer: Stopwatch around `next`, log duration or emit metrics/traces.

54. Should middleware hold per-request data in fields?
Answer: No. Use local variables or `HttpContext.Items`.

55. Can middleware be thread-safe issue source?
Answer: Yes, if instance fields are mutated across requests.

## Testing and Debugging

56. How do you unit test middleware?
Answer: Create `DefaultHttpContext`, fake `RequestDelegate`, call middleware `InvokeAsync`, assert status/body/headers.

57. How do you integration test pipeline?
Answer: Use `WebApplicationFactory`/`TestServer` and send real HTTP requests.

58. How to verify middleware order quickly?
Answer: Add temporary logs before and after `next` in each middleware.

59. How to debug branch behavior?
Answer: Log path/predicate decisions and verify which branch executes.

60. How to detect short-circuit bug?
Answer: Missing downstream behavior or logs indicates a middleware skipped `next`.

## Scenario Questions (Frequently Asked)

61. "Add request logging for all APIs but skip static files."
Strong answer: Place logging after `UseStaticFiles` or branch by path/content type.

62. "Return unified JSON error response for unhandled exceptions."
Strong answer: Add centralized exception middleware early; map exceptions to status codes.

63. "Apply middleware only for `/api` routes."
Strong answer: Use `Map("/api", branch => ...)` or conditional `UseWhen`.

64. "Only run custom middleware for authenticated users."
Strong answer: Place middleware after authn/authz and check `context.User.Identity?.IsAuthenticated`.

65. "Need DB context in middleware constructor; what to do?"
Strong answer: Use `IMiddleware` or inject scoped service into `InvokeAsync`.

66. "CORS not working on endpoints."
Strong answer: Verify middleware order with routing/endpoints and policy registration.

67. "Custom exception middleware not catching some errors."
Strong answer: Move it earlier; confirm downstream exceptions occur before response starts.

68. "Middleware runs but endpoint not hit."
Strong answer: Check short-circuit, branch conditions, and endpoint mapping order.

69. "How to attach correlation ID to all logs and responses?"
Strong answer: Early middleware adds correlation ID to `Items`, logging scope, and response header.

70. "How to conditionally bypass expensive middleware for health checks?"
Strong answer: Early path check and short-circuit or `UseWhen` branch exclusions.

## Rapid Fire (One-Liners to Memorize)

1. Middleware order is behavior.
2. `next` means "rest of pipeline".
3. `Use` can continue; `Run` ends.
4. `Map` branches by path.
5. `MapWhen` branches by predicate.
6. `UseWhen` is conditional and can rejoin.
7. `Action<T>` returns `void`.
8. `Func<T, TResult>` returns value.
9. `RequestDelegate` is `HttpContext -> Task`.
10. Startup builds pipeline; requests execute it.
