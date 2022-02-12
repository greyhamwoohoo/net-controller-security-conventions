# net-controller-security-conventions
It is common practice for developers to add or remove the [AllowAnonymous] or [Authorize] attributes from HttpMethods when developing or testing locally so the merry dance of authorization is not required. 

This is a security hazard: many, many times in my career I have seen these ad-hoc changes make it thru PRs to 'production'. It happens. And its easy to prevent using Static Analysis implemented using unit tests.

The solution is to acknowledge all insecure controllers in a separate test project to prevent accidental changes making it into production. 

## Solution
This repository shows a solution whereby a specification for insecure controllers and methods are encoded in unit tests, in a separate project, using attributes. The unit test project takes a reference to the API so that the controller configuration can be accessed using reflection. A conscious decision must be made when the controllers are made less secure by acknowledging that insecurity, using an attribute, in the unit tests. 

All insecure controller specifications must be explicitly acknowledged by attributes in unit tests. The tests run as part of the PR and will fail if the real controllers are less secure than the specification. 

CODEOWNERS can be used to add mandatory approvers when the unit tests are changed; this means that insecure changes must be approved by a qualified party thereby reducing the likelihood further of insecure changes making it to production. 

## Scenarios
These scenarios are covered:

| Scenario | Description | 
| -------- | ----------- |
| Developers remove [Authorize] from a controller | Every controller *MUST* be tagged with [Authorize] or [AllowAnonymous]<br><br>Tests: ControllerConventionTests |
| Developers change a controller from [Authorize] to [AllowAnonymous] | Every anonymous controller *MUST* be acknowledged with an attribute in the tests. <br><br>Tests: AnonymousControllerConventionTests |
| An anonymous controller has an [Authorize] method. <br><br>The developer removes [Authorize] to test localy | As HttpMethods inherit the authorization rules of their parent controller by default, commenting out [Authorize] on a method makes it insecure. Therefore, every [Authorize] method in an anonymous controller must be explicitly acknowledged. <br><br>Tests: HttpMethodConventionTests |
| An [Authorize] controller has a [AllowAnonymous] method. <br><br>The developer adds a new [AllowAnonymous] attribute for local development on a particular method but forgets to remove it | As HttpsMethods inherit the authorization rules of their parent controller by default, commenting out [AllowAnonymous] will implicitly make the methods require secure. Adding [AllowAnonymous] for local testing (and forgetting to remove it) will make the method less secure. Therefore, every [AllowAnonymous] method in an authorized controller must be explicitly acknowledged <br><br>Tests: HttpMethodContentionTests |

## Considerations
The naive solution to this problem is to require an acknowledgement of every single controller and anonymous method in the tests (secure or anonymous): this does not scale and is not a good experience. Rather: the solution in this repository prevents accidental drift of the most common scenarios. 

Personal preference: it is better to design security around controllers instead of methods. The scenarios above - whereby an authorized controller can have an anonymous method; or an anonymous controller can have an authorized method - can ideally be designed out of the system. In such a case, a test can be written to ensure that every authorized controller contains no anonymous methods; and that every anonymous controller contains no authorized methods. This is left as an exercise for the reader. 

There is a test to ensure that every single controller has either [Authorize] or [AllowAnonymous] - but not a test to enforce the same for methods. Why? To simplify local development. The common practice is usually to turn off authorization at the controller level.

## TODO:
Currently, there are no 'dynamic' tests which ensure that an Authorization is wired up after the application is built. Maybe soon. 
