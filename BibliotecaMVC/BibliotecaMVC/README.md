# Biblioteca Web Desarrollada con MVC para programación web 2

Aplicación ASP.NET Core MVC. El repositorio se actualiza cada semana con la tarea practica proporcionada. 
Adicionalmente se utiliza para la materia de Marcos de Trabajo Agil para evidenciar prácticas de calidad,

## Prácticas de calidad 
1. **Coding standards:** archivo `.editorconfig` (plantilla de Visual Studio).
   Define indentación (4 espacios), finales de línea CRLF y convenciones de C#
   (por ejemplo, interfaces con prefijo `I`, como `IAutorService`).
2. **Pull Request y code review:** los estándares y esta documentación se
   integraron por una rama (`calidad/editorconfig-readme`) y un PR hacia `main`,
   con comentarios de revisión antes del merge.
## Qué problema evitan
- El EditorConfig evita mezclar tabuladores y espacios y el retrabajo de
  “arreglar estilo” al final del curso.
- El PR evita integrar tarde un paquete enorme de cambios (integración
  “Big Bang”), cuando corregir es más difícil.
## Relación con la materia de Marcos de trabajo agil
Se prefiere integrar seguido (un commit o un PR por práctica) en lugar de
acumular semanas. En el mismo proyecto se aplicó IoC/DI: `AutorController`
depende de `IAutorService`, registrado en `Program.cs`. Cambiar la
implementación no exige reescribir el controlador: menos acoplamiento y
menos retrabajo.
## Cómo ejecutar
Abrir la solución en Visual Studio y ejecutar con F5.