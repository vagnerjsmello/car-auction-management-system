
---

# Car Auction Management System

This repository contains the solution for the **Car Auction Management System**, a system to manage vehicle auctions. The system allows you to:

1. **Add vehicles** to the inventory (Sedan, SUV, Hatchback, Truck), ensuring each vehicle has a unique identifier.
2. **Search for vehicles** by type, manufacturer, model, or year.
3. **Start and close auctions** for vehicles, making sure only one auction is active per vehicle.
4. **Place bids** in active auctions, ensuring that a new bid is higher than the current bid.
5. **Handle errors** using specific exceptions (for example, `VehicleNotFoundException`, `InvalidBidException`, etc.) and centralised validations (using `FluentValidation` and **NotificationPattern**).

The solution was implemented using principles of **Clean Architecture** and **Domain-Driven Design (DDD)**, applying Object-Oriented patterns.

---

## Solution Structure

The solution is divided into four main projects:

```
CarAuctionManagementSystem.sln
├── CAMS.Application
├── CAMS.Domain
├── CAMS.Infrastructure
└── CAMS.Tests
```

---

## 1. CAMS.Application

This project is responsible for CQRS (Commands and Queries) using **MediatR**.  
Each command or query has its request/command, response, handler, and optionally a validator (using **FluentValidation**).

### Example Folder Structure

```
CAMS.Application
└── Commands
    ├── Auctions
    │   ├── CloseAuction
    │   │   ├── CloseAuctionCommand.cs
    │   │   ├── CloseAuctionCommandHandler.cs
    │   │   ├── CloseAuctionCommandValidator.cs
    │   │   └── CloseAuctionResponse.cs
    │   ├── PlaceBid
    │   │   ├── PlaceBidCommand.cs
    │   │   ├── PlaceBidCommandHandler.cs
    │   │   ├── PlaceBidCommandValidator.cs
    │   │   └── PlaceBidResponse.cs
    │   └── StartAuction
    │       ├── StartAuctionCommand.cs
    │       ├── StartAuctionCommandHandler.cs
    │       ├── StartAuctionCommandValidator.cs
    │       └── StartAuctionResponse.cs
    └── Vehicle
        └── Create Vehicle
            ├── CreateVehicleCommand.cs
            ├── CreateVehicleCommandHandler.cs
            ├── CreateVehicleCommandValidator.cs
            ├── CreateVehicleRequest.cs
            └── CreateVehicleResponse.cs
└── Queries
    ├── Auctions
    │   ├── GetAuction
    │   │   ├── GetAuctionQuery.cs
    │   │   ├── GetAuctionQueryHandler.cs
    │   │   └── GetAuctionResponse.cs
    │   └── SearchAuctions
    │       ├── SearchAuctionsQuery.cs
    │       ├── SearchAuctionsQueryHandler.cs
    │       └── SearchAuctionsResponse.cs
    └── Vehicles
        ├── SearchVehiclesQuery.cs
        ├── SearchVehiclesQueryHandler.cs
        └── SearchVehiclesResponse.cs
└── Common
        └── OperationResult.cs
```

**Responsibilities:**

- **Commands:** They perform write operations. They check the data and work with the domain. They also send Domain Events using an injected DomainEventPublisher.
- **Queries:** They let you read and search for information. They return organized responses.
- **OperationResult:** Our commands and queries now return an OperationResult. This supports the Notification Pattern by gathering all validation errors and success data into one unified response.

---

## 2. CAMS.Domain

This project defines the business rules and core entities, independent of infrastructure.

### Folder Structure

```
CAMS.Domain
├── Entities
│   ├── AggregateRoot.cs
│   ├── Auction.cs
│   ├── Bid.cs
│   ├── Hatchback.cs
│   ├── Sedan.cs
│   ├── SUV.cs
│   ├── Truck.cs
│   └── Vehicle.cs
├── Enums
│   ├── AuctionStatus.cs
│   └── VehicleType.cs
├── Events
│   ├── AuctionClosedEvent.cs
│   ├── AuctionStartedEvent.cs
│   ├── BidPlacedEvent.cs
│   ├── DomainEvent.cs
│   └── IHasDomainEvents.cs
├── Exceptions
│   ├── AuctionAlreadyActiveException.cs
│   ├── AuctionNotFoundException.cs
│   ├── InvalidBidException.cs
│   ├── VehicleAlreadyExistsException.cs
│   └── VehicleNotFoundException.cs

├── Factories
│   └── VehicleFactory.cs
└── Repositories
    ├── IAuctionRepository.cs
    └── IVehicleRepository.cs
```

**Key Points:**

- **Entities:**  
  - `Vehicle` is an abstract class with specific subclasses (Sedan, SUV, Hatchback, Truck) that define unique attributes.  
  - `Auction` manages bids (using the `Bid` entity), the auction status (Active/Closed), and registers Domain Events (such as `AuctionStartedEvent`, `BidPlacedEvent`, and `AuctionClosedEvent`) in its `DomainEvents` collection.

- **Factory Method:**  
  `VehicleFactory` centralises the creation of `Vehicle` instances based on the `VehicleType`.

- **Exceptions and Notification Pattern:**  
  Custom exceptions (e.g. `AuctionAlreadyActiveException`, `AuctionNotFoundException`, `InvalidBidException`, `VehicleAlreadyExistsException`, `VehicleNotFoundException`) are standardised to centralise error handling. 
  We use `FluentValidation` to check user input and create an OperationResult. This gathers all errors into one response using the **Notification Pattern**, avoiding immediate exception throws.
  
- **Events:**  
  Domain Events are recorded in the entities and later dispatched by the Application/Infrastructure layer.


---

## 3. CAMS.Infrastructure

This project provides concrete implementations for persistence and event dispatching, integrating the domain with infrastructure details.

### Folder Structure

```
CAMS.Infrastructure
├── Events
│   ├── DomainEventPublisher.cs
│   ├── FakeDomainEventDispatcher.cs
│   ├── IDomainEventDispatcher.cs
│   └── IDomainEventPublisher.cs
└── Repositories
    ├── InMemoryAuctionRepository.cs
    └── InMemoryVehicleRepository.cs
```

**Responsibilities:**

- **In-Memory Repositories:**  
  These implement the contracts defined in `CAMS.Domain.Repositories` using in-memory collections (for example, `ConcurrentDictionary`).

- **Event Dispatching:**  
  - `FakeDomainEventDispatcher` simulates the sending of Domain Events (for example, to a Service Bus).  
  - `DomainEventPublisher`, via the `IDomainEventPublisher` interface, collects the Domain Events recorded by the entities and dispatches them, clearing the list afterwards.

---

## 4. CAMS.Tests

This project contains unit tests to ensure the system’s quality across all layers.

### Folder Structure

```
CAMS.Tests
├── Application
│   ├── Auctions
│   │   ├── CloseAuctionCommandHandlerTests.cs
│   │   ├── GetAuctionQueryHandlerTests.cs
│   │   ├── PlaceBidCommandHandlerTests.cs
│   │   ├── SearchAuctionsQueryHandlerTests.cs
│   │   └── StartAuctionCommandHandlerTests.cs
│   └── Vehicles
│       ├── CreateVehicleCommandHandlerTests.cs
│       └── SearchVehiclesQueryHandlerTests.cs
└── Domain
    ├── AuctionRepositoryTests.cs
    ├── AuctionTests.cs
    ├── VehicleFactoryTests.cs
    └── VehicleRepositoryTests.cs
```

**Test Responsibilities:**

- **Application Tests:**  
  Test the command and query handlers by checking constructors (null dependency checks), business logic (success and error scenarios), and validations (using FluentValidation.TestHelper).

- **Domain Tests:**  
  Validate the internal behaviour of entities, repositories, and factories.

---

## Libraries Used

- **.NET 8 LTS:** Development platform (Long Term Support).
- **MediatR:** Implements the CQRS pattern for commands and queries.
- **FluentValidation:** Validates commands and queries.
- **xUnit:** Testing framework.
- **Moq:** For dependency mocking.
- **FluentAssertions:** Provides fluent assertions in tests.
- **System.Collections.Concurrent:** Used in in-memory repositories to ensure safety in concurrent scenarios.

---

## Architecture and Object-Oriented Patterns

- **Clean Architecture / DDD:**  
  The separation between **Domain**, **Application**, **Infrastructure**, and **Tests** ensures that the domain remains independent of infrastructure details, and that business logic is centralised in the entities.

- **Object-Oriented Principles:**  
  - **Inheritance:** `Vehicle` is an abstract class with specific subclasses (Sedan, SUV, Hatchback, Truck).  
  - **Encapsulation:** `Auction` manages its bids and auction status internally, exposing methods to update its state consistently.  
  - **Factory Method:** `VehicleFactory` creates correct instances of `Vehicle` based on `VehicleType`, centralising creation logic.
  - **Domain Events:** Entities record events (e.g. `AuctionStartedEvent`, `BidPlacedEvent`, `AuctionClosedEvent`) in the `DomainEvents` collection. Event dispatching is handled by the Application/Infrastructure layer via `DomainEventPublisher`.

- **CQRS with MediatR:**  
  A clear separation between commands (write operations) and queries (read operations) makes the system easier to maintain and evolve.

- **Single Responsibility Principle:**  
  Each layer and class has a well-defined responsibility. For example, the domain only records events, while the Application layer (through `DomainEventPublisher` in Infrastructure) handles the dispatch of these events.

- **Custom Exceptions and Validations:**  
  Custom exceptions have been standardised to centralise error messages and simplify error handling. Validations are implemented with **FluentValidation** and tested with **FluentValidation.TestHelper** to ensure data consistency.

  **Notification Pattern**
  Instead of throwing exceptions immediately for validation errors, our system collects errors into an `OperationResult`. This allows all validation errors to be returned together in a single, unified response, making error handling more efficient and user-friendly.

---

## Key Tests and Requirements Coverage

Unit tests have been created to cover the system’s critical scenarios:

- **CreateVehicleCommandHandlerTests:**  
  - **Handle_ShouldCreateVehicleSuccessfully_WhenCommandIsValid:** Checks vehicle creation with valid data.  
  - **Handle_ShouldReturnFailureResult_WhenValidationFailsForCreateVehicleCommand:** Ensures that invalid data is inserted into the response via Notification Pattern without having to throw an exception.  
  - **Handle_ShouldThrowVehicleAlreadyExistsException_WhenVehicleWithSameIdExists:** Tests duplicate detection and throws the correct exception.

- **StartAuctionCommandHandlerTests:**  
  - **Handle_ShouldCreateAuctionSuccessfully_WhenCommandIsValid:** Confirms that auctions start correctly for valid vehicles without an active auction.  
  - **Handle_ShouldThrowVehicleNotFoundException_WhenVehicleDoesNotExist:** Validates that starting an auction for a non-existent vehicle throws the proper exception.  
  - **Handle_ShouldThrowAuctionAlreadyActiveException_WhenActiveAuctionExists:** Ensures that if an active auction already exists, the command throws the appropriate exception.

- **PlaceBidCommandHandlerTests:**  
  - **Handle_ShouldPlaceBidSuccessfully_WhenBidIsValid:** Checks that a valid bid is processed and the auction is updated with the new bid.  
  - **Handle_ShouldThrowAuctionNotFoundException_WhenAuctionDoesNotExist:** Ensures that bidding on a non-existent auction throws the correct exception.  
  - **Handle_ShouldThrowInvalidBidException_WhenBidIsNotHigherThanCurrent:** Confirms that a bid that is not higher than the current bid raises the proper exception.  
  - **Validation Tests:** Check each rule individually (for `AuctionId`, `BidAmount`, and `BidderId`).

- **CloseAuctionCommandHandlerTests:**  
  - **Handle_ShouldCloseAuctionSuccessfully_WhenAuctionIsActive:** Verifies that an active auction is closed correctly and the Domain Events are fired.  
  - **Handle_ShouldThrowAuctionNotFoundException_WhenAuctionDoesNotExist:** Ensures that closing a non-existent auction throws the correct exception.

- **GetAuctionQueryHandlerTests and SearchAuctionsQueryHandlerTests:**  
  Validate reading and filtering operations for auctions, ensuring the returned data meets the expected conditions.

These tests are essential to ensure that:
- **Adding vehicles** and checking for duplicates work correctly.
- **Starting and closing auctions** occur only for valid vehicles, with proper error handling and event dispatching.
- **Placing bids** is allowed only if the auction is active and the new bid is higher than the current bid.
- All **validations** and **error treatments** meet the system requirements.

---

## Execution Process

### Running Tests

1. Open a terminal in the project root.
2. Navigate to the tests folder:
   ```bash
   cd src/CAMS.Tests
   ```
3. Run:
   ```bash
   dotnet test
   ```
   This will execute all 97 tests and ensure system quality.

### Code Coverage

To generate the code coverage report:

- **Using the Bash Script (`run_coverage.sh`) – for Linux/macOS/Windows with Git Bash or WSL:**  
  Simply run:
  ```bash
  ./run_coverage.sh
  ```

- **Using the Batch Script (`run_coverage.bat`) – for Windows:**  
  Simply run:
  ```batch
  run_coverage.bat
  ```

- **Manual Instructions:**
  1. Run:
     ```bash
     dotnet test --collect:"XPlat Code Coverage"
     ```
  2. Navigate to the `TestResults` folder, open the GUID subfolder, and run:
     ```bash
     reportgenerator "-reports:coverage.cobertura.xml" "-targetdir:coveragereport"
     ```
  3. The file `index.html` will be created in `coveragereport`.

---

## Conclusion

The **Car Auction Management System** solution is designed to be modular, testable, and extensible, meeting the problem requirements and allowing future integrations with real persistence or external services. The layered architecture (Domain, Application, Infrastructure, and Tests) and the use of Object-Oriented patterns (inheritance, encapsulation, factory method) ensure that the code is clean and easy to maintain.

If you have any questions or suggestions, please feel free to open an issue or submit a pull request!

--- 

