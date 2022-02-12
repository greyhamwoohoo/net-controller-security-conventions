# net-controller-security-conventions
It is common practice for developers to add or remove the [AllowAnonymous] or [Authorize] attributes from HttpMethods when developing or testing locally so the merry dance of authorization is not required. 

This is a security hazard: many, many times in my career I have seen these ad-hoc changes make it thru PRs to 'production'. It happens. And its easy to prevent using Static Analysis implemented using unit tests.

The solution is to acknowledge all insecure controllers in a separate test project to prevent accidental changes making it into production. 

## Solution
This repository shows a solution whereby a specification for insecure controllers and methods are encoded (via 'acknowledgements') in unit tests, in a separate project, using attributes. The unit test project takes a reference to the API project so that the controllers can be accessed using reflection. A conscious decision must be made when the controllers are made less secure by acknowledging that insecurity, using an attribute, in the unit tests. 

Unit tests should run as part of the PR and will fail if the real controllers are less secure than the specification. 

CODEOWNERS can be used to add mandatory approvers when the unit tests are changed; this means that insecure changes must be approved by a qualified party thereby reducing the likelihood further of insecure changes making it to production. 

## Implementing
A sample API and UnitTests are provided in the src/ folder. Code is in .Net Core. Build and run. 

## Scenarios
The following scenarios are covered by the tests: the implementation is likely simpler than you would think by looking at this!

### New Scenarios
The following scenarios will arise during new development:
| Scenario | Description | 
| -------- | ----------- |
| Developers add a new controller but do not specify security | Every controller *MUST* be tagged with [Authorize] or [AllowAnonymous]<br><br>Tests: ControllerConventionTests |
| Developers add a new [AllowAnonymous] controller | The new controller must be acknowledged in the tests using [AcknowledgeAnonymousController] attribute. <br><br>CODEOWNERS can be used so that a security owner is aware of anonymous functionality getting onto the code base. CODEOWNERS applies to all scenarios here so will not be repeated again. |
| Developers add a new [Authorize] controller | No impact | 
| Developers add an [Authorize] method to an anonymous controller | The authorized methods must be acknowledged in the tests with a [AcknowledgeAuthorizedHttpMethod] attribute. <br><br>This acknowledgement will prevent the accidential removal of the authorization attribute in future (which would make the method inherit the insecurity of its parent controller) |
| Developers add an [AllowAnonymous] method to an authorize controller | The anonymous method must be acknowledged in the tests with a [AcknowledgeAnonymousHttpMethod] attribute. <br><br>This acknowledgement will prevent insecure methods in otherwise secure controllers accidentally making it to production |

### Change/Modification Scenarios
The following scenarios will arise when modifying existing code; or developing locally:
| Scenario | Description | 
| -------- | ----------- |
| Developers remove [Authorize] from a controller | Every controller *MUST* be tagged with [Authorize] or [AllowAnonymous]<br><br>Tests: ControllerConventionTests |
| Developers change a controller from [Authorize] to [AllowAnonymous] | Every anonymous controller *MUST* be acknowledged with an attribute in the tests. <br><br>Tests: AnonymousControllerConventionTests |
| An anonymous controller has an [Authorize] method. <br><br>The developer removes [Authorize] to test locally | As HttpMethods inherit the authorization rules of their parent controller by default, commenting out [Authorize] on a method makes it insecure. Therefore, every [Authorize] method in an anonymous controller must be explicitly acknowledged. <br><br>Tests: HttpMethodConventionTests |
| An [Authorize] controller has a [AllowAnonymous] method. <br><br>The developer adds a new [AllowAnonymous] attribute for local development on a particular method but forgets to remove it | As HttpMethods inherit the authorization rules of their parent controller by default, commenting out [AllowAnonymous] will implicitly make the methods require secure. Adding [AllowAnonymous] for local testing (and forgetting to remove it) will make the method less secure. Therefore, every [AllowAnonymous] method in an authorized controller must be explicitly acknowledged <br><br>Tests: HttpMethodContentionTests |

### Deletion Scenarios
The following scenarios will arise when deleting code (a rename of a method will likely appear as a deletion depending on how you refactor):
| Scenario | Description | 
| -------- | ----------- |
| An acknowledged anonymous controller is deleted | Every acknowledged controller MUST exist. <br><br>Tests: AnonymousControllerConventionTests |
| A [AllowAnonymous] method in a [Authorize] controller is deleted or renamed | Every acknowledged anonymous method must exist. <br><br>Tests: HttpMethodConventionTests |
| A [Authorize] method in a [AllowAnonymous] controller is deleted or renamed | Every acknowledged authorized method must exist. <br><br>Tests: HttpMethodConventionTests |
| An [Authorize] controller is deleted | No impact |

## Considerations
The naive solution to this problem is to require an acknowledgement of every single controller and anonymous method in the tests (secure or anonymous): this does not scale and is not a good experience. Rather: the solution in this repository prevents accidental drift of the most common scenarios. 

Personal preference: it is better to design security around controllers instead of methods. The scenarios above - whereby an authorized controller can have an anonymous method; or an anonymous controller can have an authorized method - can ideally be designed out of the system. In such a case, a test can be written to ensure that every authorized controller contains no anonymous methods; and that every anonymous controller contains no authorized methods. This is left as an exercise for the reader. 

There is a test to ensure that every single controller has either [Authorize] or [AllowAnonymous] - but not a test to enforce the same for methods. Why? To simplify local development. The common practice is usually to turn off authorization at the controller level locally.

## TODO:
Currently, there are no 'dynamic' tests which ensure that an Authorization is wired up after the application is built. Maybe soon. 
