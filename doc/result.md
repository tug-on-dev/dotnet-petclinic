---
author: Documentation Specialist Agent
description: Recommendations for design, performance, and async improvements in the ASP.NET Core Pet Clinic application
last_changed: 2025-01-27
---

# Pet Clinic Application Improvements

## Table of Contents

- [Overview](#overview)
- [Design Improvements](#design-improvements)
- [Performance Improvements](#performance-improvements)
- [Non-Blocking and Async Improvements](#non-blocking-and-async-improvements)
- [Quick Win Checklist](#quick-win-checklist)

## Overview

This document provides actionable recommendations to improve the Pet Clinic ASP.NET Core MVC application. The recommendations focus on three key areas: architectural design patterns, performance optimizations, and async/non-blocking operations. Each suggestion is tied to specific files and components in the current codebase.

> [!NOTE]
> These improvements are ordered from minimal disruption (quick wins) to larger architectural changes. Start with performance and async improvements before tackling major design refactoring.

## Design Improvements

The current architecture couples controllers directly to `PetClinicDbContext`, resulting in fat controllers with mixed responsibilities (HTTP handling, business logic, and data access). This makes testing difficult and violates separation of concerns.

**Recommended Actions:**

- **Introduce a Repository Pattern or Application Services Layer**
  - Create repository interfaces (`IOwnerRepository`, `IPetRepository`) to encapsulate data access logic currently in controllers
  - Move complex queries from `OwnerController` and `PetController` into dedicated repository implementations
  - Example: Extract `OwnerController.Index` query logic (with `Include` for navigation properties) into `OwnerRepository.GetPaginatedOwnersAsync()`
  - Benefits: Improved testability, easier to mock data access, centralized query logic

- **Consider MediatR for CQRS Pattern (Optional)**
  - For larger applications, implement Command/Query separation using MediatR
  - Example: `CreatePetCommand`, `GetOwnersByNameQuery` as discrete request handlers
  - Benefits: Clear separation of read/write operations, easier to add cross-cutting concerns (validation, logging)

- **Extract Business Logic into Domain Services**
  - Move validation and business rules out of controllers
  - Example: Pet type validation, owner-pet relationship management
  - Keep controllers thin, focused only on HTTP request/response handling

- **Improve Dependency Injection Structure**
  - Register repositories/services with appropriate lifetimes in `Program.cs`
  - Use interface-based dependencies to improve testability
  - Example: `services.AddScoped<IOwnerRepository, OwnerRepository>()`

> [!TIP]
> Start with the repository pattern for frequently-used entities (Owner, Pet) before expanding to all entities. This allows incremental migration without disrupting the entire codebase.

## Performance Improvements

The application currently uses tracked queries for all reads, fetches reference data on every request, and implements synchronous pagination. These patterns create unnecessary overhead and memory pressure.

**Recommended Actions:**

- **Use AsNoTracking for Read-Only Queries**
  - Apply `.AsNoTracking()` to all queries in read-only scenarios (e.g., `OwnerController.Index`, `PetController.Index`)
  - Tracking entities is only needed when updating; read-only views don't require change tracking overhead
  - Example: `_context.Owners.AsNoTracking().Include(o => o.Pets).ToListAsync()`
  - Expected benefit: 20-30% reduction in memory allocation for list views

- **Implement Caching for Reference Data**
  - Cache `PetTypes` using `IMemoryCache` since they rarely change
  - Currently fetched from database on every request in `PetController.Create` and `PetController.Edit`
  - Example: Cache with 1-hour sliding expiration, invalidate on PetType updates
  - Benefit: Reduces database round-trips for static lookup data

- **Optimize Pagination Queries**
  - The `PaginatedList.Create` method executes separate `Count()` and `ToList()` queries
  - Consider batching or caching total count for frequently accessed pages
  - For large datasets, use keyset pagination instead of offset-based (more efficient, no full count needed)

- **Add Database Indexes**
  - Review query patterns and add indexes on frequently filtered/sorted columns
  - Example: Index on `Owner.LastName` for search queries
  - Example: Index on `Pet.OwnerId` for join operations (if not already present via FK)
  - Use SQL query profiling to identify slow queries

- **Projection for List Views**
  - Instead of loading full entities, project only needed fields for list views
  - Example: `Select(o => new OwnerListViewModel { Id = o.Id, FullName = o.FirstName + " " + o.LastName })`
  - Reduces data transfer and memory footprint

> [!IMPORTANT]
> Always measure performance before and after optimizations. Use tools like MiniProfiler or Application Insights to identify actual bottlenecks rather than premature optimization.

## Non-Blocking and Async Improvements

The application uses synchronous database operations in critical paths, including blocking initialization and synchronous pagination. This limits scalability and can cause thread pool starvation under load.

**Recommended Actions:**

- **Replace EnsureCreated with Migrations**
  - Current `DbInitializer.Initialize` uses synchronous `context.Database.EnsureCreated()` at startup
  - Replace with proper EF Core migrations (`dotnet ef migrations add InitialCreate`)
  - Run migrations asynchronously: `context.Database.MigrateAsync()` in `Program.cs`
  - Benefits: Better for production, supports schema evolution, non-blocking startup

- **Implement Async Seeding Strategy**
  - Move seed data logic to separate async initialization or use a startup background service
  - Example: Create `IHostedService` implementation for background seeding after app starts
  - Prevents blocking the application startup pipeline
  - File to modify: `Data/DbInitializer.cs` and `Program.cs`

- **Convert PaginatedList to Async**
  - Replace synchronous `Count()` and `ToList()` in `PaginatedList.Create` with `CountAsync()` and `ToListAsync()`
  - Create `CreateAsync` method: `public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)`
  - Update all controller actions to `async Task<IActionResult>` and await the pagination call
  - Files to modify: `PaginatedList.cs`, `OwnerController.cs`, `PetController.cs`

- **Make All Database Operations Async**
  - Replace `context.SaveChanges()` with `await context.SaveChangesAsync()`
  - Use `FirstOrDefaultAsync()`, `ToListAsync()`, `AnyAsync()` instead of synchronous counterparts
  - Ensure all controller actions interacting with database are `async Task<IActionResult>`
  - Search codebase for synchronous EF methods and convert systematically

- **Add Cancellation Token Support**
  - Pass `CancellationToken` from controller actions to repository/service methods
  - Example: `GetOwnersByNameAsync(string searchString, CancellationToken cancellationToken = default)`
  - Allows graceful cancellation of long-running queries if client disconnects
  - EF Core async methods accept cancellation tokens

> [!WARNING]
> Avoid mixing sync and async code (e.g., `.Result` or `.Wait()` on async tasks). This can cause deadlocks in ASP.NET Core. Always use `await` for async operations.

## Quick Win Checklist

These are low-effort, high-impact changes you can implement immediately:

### Immediate (< 1 hour)
- [ ] Add `.AsNoTracking()` to all read-only queries in list/detail views
- [ ] Convert `PaginatedList.Create` to `CreateAsync` with `CountAsync()` and `ToListAsync()`
- [ ] Make all controller actions that touch the database `async Task<IActionResult>`

### Short-term (1-2 days)
- [ ] Implement `IMemoryCache` for `PetTypes` reference data
- [ ] Replace `EnsureCreated()` with EF Core migrations
- [ ] Make `DbInitializer.Initialize` async and call via `MigrateAsync()`
- [ ] Add database indexes on commonly queried fields

### Medium-term (1 week)
- [ ] Extract repository interfaces for Owner and Pet entities
- [ ] Move complex queries from controllers to repository implementations
- [ ] Add `CancellationToken` parameters to async methods
- [ ] Create ViewModels for list views to enable projection queries

### Long-term (2+ weeks)
- [ ] Implement comprehensive repository pattern for all entities
- [ ] Consider application services layer for business logic
- [ ] Evaluate MediatR/CQRS if application grows in complexity
- [ ] Add query profiling and performance monitoring

## Related Resources

- [EF Core Performance Best Practices](https://learn.microsoft.com/en-us/ef/core/performance/)
- [ASP.NET Core Performance Best Practices](https://learn.microsoft.com/en-us/aspnet/core/performance/performance-best-practices)
- [Async Guidance for ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/best-practices)
