# CurrencyX

Project used to track the history of exchange rates (base currency EUR) and to convert EUR to desired currency.

## Prerequisites

Before you begin, make sure that the following tools are installed on your local machine:

- **Docker**: Docker Compose requires Docker to be installed and running on your local machine.

  - [Download Docker Desktop](https://www.docker.com/products/docker-desktop) for **Windows** and **macOS**.
  - For **Linux** distributions, follow the installation instructions from the [Docker Documentation](https://docs.docker.com/engine/install/).

- **Git**: If you need to clone the repository, you will also need Git. You can download it from [here](https://git-scm.com/downloads).

## Setup Instructions

1. **Install Docker Desktop**:
   Download and install Docker Desktop for Windows or macOS from the official [Docker website](https://www.docker.com/products/docker-desktop). Follow the installation instructions provided there.

   For **Linux** users, install Docker by following the instructions from [Docker Engine Installation](https://docs.docker.com/engine/install/).

2. **Clone the Repository**:
   Clone the repository to your local machine. Open a terminal and run the following commands:

   ```bash
   git clone https://github.com/luc1an24/CurrencyX.git
   cd CurrencyX

3. **Run Docker Compose**:
   With Docker running, you can use Docker Compose to start up all the necessary services. To do so, run:

   ```bash
   docker-compose up

4. **Verify the Services**:
   Once the services are up, verify that they are running correctly. Navigate to application webpage: [http://localhost:4200](http://localhost:4200).
