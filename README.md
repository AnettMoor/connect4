# EF

## Tooling
dotnet tool install --global dotnet-ef

dotnet tool update --global dotnet-ef

## Migrations
// run from the solution folder

// add new migration

dotnet ef migrations add --startup-project WebApp --project DAL Initial

// apply migrations

dotnet ef database update --startup-project WebApp --project DAL
//?
dotnet ef database update --startup-project WebApp --project DAL --context AppDbContext


// drop the database

dotnet ef database drop --startup-project WebApp --project DAL

# Asp.net

## Tooling
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet tool update --global dotnet-aspnet-codegenerator

install from nuget:
- Microsoft.VisualStudio.Web.CodeGeneration.Design
- Microsoft.EntityFrameworkCore.SqlServer

// webapp packages
<ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="10.0.0"/>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.0"/>
    <PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="10.0.0"/>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.0"/>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.0"/>
    <PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="9.0.0" />
</ItemGroup>


## Scaffolding
// run from webapp folder
cd WebApp
dotnet aspnet-codegenerator razorpage -m Category -dc AppDbContext -udl -outDir Pages/Categories --referenceScriptLibraries
dotnet aspnet-codegenerator razorpage -m Priority -dc AppDbContext -udl -outDir Pages/Priorities --referenceScriptLibraries
dotnet aspnet-codegenerator razorpage -m ToDo -dc AppDbContext -udl -outDir Pages/ToDos --referenceScriptLibraries

## Update _layout.cshtml
add links
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/ToDos/Index">Todos</a>
</li>
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/Priorities/Index">Priorities</a>
</li>
<li class="nav-item">
    <a class="nav-link text-dark" asp-area="" asp-page="/Categories/Index">Categories</a>
</li>

## fix ui problems
//BASIC UI FIXES
//wrong css class in css options.
Replace "form-control" with "form-select"

<select asp-for="ToDo.CategoryId"class="form-select" asp-items="ViewBag.CategoryId"></select>

//Add some bottom margin in site.css
.form-group {
margin-bottom: 1em;
}