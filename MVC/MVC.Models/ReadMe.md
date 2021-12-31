# MVC.Models
This library should contain Kentico Xperience agnostic classes (DTO / Data Transfer Objects) and interfaces.

The Interfaces represent the data retrieve/writing/deleting logic that the MVC site will use.

They should return the Xperience agnostic classes and receive them as well. 

For these reasons, this library does not reference Kentico.Xperience.Libraries, nor should it ever.

Your MVC.Libraries class library will be responsible for implementing these interfaces and converting the Kentico Xperience objects into the agnostic DTOs returned in the interfaces.