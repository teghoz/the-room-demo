### Questions
#### How do you document code?
Although some on the development side of the aisle would argue that good documentation would lead to job insecurity :laughing:, Good Documentation is vital, especially when speed to market and scalability is of utmost importance. Imagine a new resource joining, and that person has to navigate through the existing code base, for example. 

Markdowns are one way I achieve this :heavy_check_mark:. There are other tools like confluence too. My current organization uses this. One straightforward way to also do this is to stick with conventions. Once everyone is familiar with a chosen pattern, you have already documented your code without typing extra information.

#### What are your thoughts on unit testing?
As systems begin to scale, the introduction of new code has the potential to introduce side effects. Side effects that one might not be thinking about when the new addition is commited to the codebase. Unit testing helps reduce these problems - regressions :boom: :exclamation:. 

Furthermore, it fosters code quality :+1:. From my experience, my best benefit is when debugging code. It accelerates the process of getting to the bottom of a potential issue(s). 

Developers are wired to solve problems. QAs, on the other hand, are wired to find problems; that mismatch always causes problems, affecting how well developers write tests. So if we start from the behaviors and walk backward to the solutions, it reduces unnecessary back and forth with QAs, increases productivity and code quality.

Finally, TDD has its challenges. E.g., It is subject to the developer's scope of understanding as regards side effects and usability. Hence, I think BDD is the direction to move forward with as it takes the best of DDD and TDD.

#### What design pattern have you used in your project?
I have used a couple of patterns in different areas. E.g. 
- Repository Pattern.
- Decorator Pattern.
- Singletons.
- Observer Pattern - with the messaging on the microservice approach.
- Strategy Pattern and probably a few more.
#### What are the most important performance issues in ASP.Net web applications?
There are several issues I have experienced in the past. 
- Entity Framework Abuse
- Garbage collection CPU consumption which tends to slow down apps.
- IIS server impediments.



#### What do you think about Typescript?
Since I come from a .NET and static typing background, I think very highly of it :raised_hands: :clap: :muscle:. However,  it can be very unpleasant :tired_face: when using it, especially for those who come from an older ESMASCRIPT ES3 > version where you had the freedoms to code dynamically.

However, for large codebases, you want a way to catch issues, errors, and exceptions at compile time. Typescript provides you that luxury :rocket:. Additionally, it allows for a more profound use of OOP concepts.

#### What are the most important performance issues in React web applications?
Well, one issue that I have experience is the flawed architecture of components. It is not precisely a react problem per se, but since they do not have strict opinions on the solution, it causes problems. I don't know why they are yet to adopt the event emission like Vue and Angular. This uncertainty leads to prop drooling and makes the user-experience bad as objects keep moving up and down the tree. 

There is always the question you need to ask every time, when is large *LARGE*?. The idea behind the use of components is reuse. However, it should be used in code organization which helps with readability. If a component is too large, it should be further decomposed regardless of whether it would be reused.