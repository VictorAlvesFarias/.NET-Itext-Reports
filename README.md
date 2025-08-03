# .NET iText Reports

## Overview

O .NET iText Reports é uma solução robusta e escalável para geração de relatórios PDF e planilhas Excel utilizando .NET 8.0, iText7, Razor Components, ClosedXML e EPPlus. Desenvolvido com arquitetura modular, oferece flexibilidade na criação de relatórios estruturados com cabeçalho, corpo e rodapé personalizáveis, além de planilhas Excel com formatação avançada. Com foco em performance e reutilização de código, utiliza templates Razor para renderização de conteúdo HTML e bibliotecas especializadas para manipulação de documentos, tornando-o ideal para aplicações empresariais que necessitam de relatórios profissionais e customizáveis em múltiplos formatos.

## Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Usage](#usage)
- [Main Features](#main-features)
  - [BaseReport Architecture](#basereport-architecture)
  - [BaseSpreadsheet Architecture](#basespreadsheet-architecture)
  - [Razor Templates](#razor-templates)
  - [API Endpoints](#api-endpoints)
  - [Configuration](#configuration)
- [Development Guide](#development-guide)
  - [Creating New Reports](#creating-new-reports)
  - [Creating New Spreadsheets](#creating-new-spreadsheets)
  - [Customization](#customization)
  - [Debugging](#debugging)

## Project Structure

```
.NET-Itext-Reports/
├── Api/                          # API Web principal
│   ├── Controllers/              # Controladores da API
│   │   └── ReportsController.cs
│   ├── Ioc/                      # Configuração de DI
│   │   └── NativeInjectorConfig.cs
│   ├── Program.cs               # Ponto de entrada da aplicação
│   ├── appsettings.json         # Configurações da aplicação
│   └── Api.csproj              # Projeto da API
├── Infra.Reports/               # Biblioteca de relatórios
│   ├── Reports/                 # Implementações de relatórios
│   │   ├── BaseReport.cs        # Classe base para relatórios PDF
│   │   ├── BaseSpreadsheet.cs   # Classe base para planilhas Excel
│   │   └── RenderMessageReport.cs
│   ├── Helpers/                 # Event handlers para PDF
│   │   ├── HeaderEventHandler.cs
│   │   └── FooterEventHandler.cs
│   ├── Extensions/              # Extensões personalizadas
│   │   └── ItextExtensions.cs
│   ├── Dtos/                    # Data Transfer Objects
│   │   └── Document/
│   │       └── Margins.cs
│   └── Infra.Reports.csproj    # Projeto da infraestrutura
└── Infra.Reports.Razor/         # Templates Razor
    ├── Reports/                 # Componentes Razor
    │   └── RenderMessage/
    │       ├── RenderMessage.razor
    │       ├── RenderMessageHeader.razor
    │       └── RenderMessageFooter.razor
    └── Infra.Reports.Razor.csproj
```

## Getting Started

### Prerequisites

Before getting started with .NET iText Reports, ensure your runtime environment meets the following requirements:

- **.NET 8.0 SDK:** https://dotnet.microsoft.com/download/dotnet/8.0
- **Visual Studio 2022** or **VS Code**
- **Knowledge of C# and Razor Components**

### Installation

Install .NET iText Reports using one of the following methods:

**Build from source:**

Clone the repository:
```sh
git clone <url-do-repositorio>
cd .NET-Itext-Reports
```

### Run dev mode

Install the project dependencies:

```sh
dotnet restore
```

Run the application using the following command:

```sh
cd Api
dotnet run
```

### Usage

Access the application:

- **API**: https://localhost:7001
- **Swagger**: https://localhost:7001/swagger

## Main Features

### BaseReport Architecture

The BaseReport class provides a foundation for all PDF reports in the application. It handles the PDF generation process with a modular approach, separating header, body, and footer rendering.

**Key Methods:**

- **Build()**: Main method for PDF construction with configurable margins and page sizes
- **Header()**: Virtual method for header rendering
- **Body()**: Virtual method for body content rendering
- **Footer()**: Virtual method for footer rendering
- **Render<T>()**: Renders Razor components with parameter injection

```csharp
public class ReportExample
{
    //...

    protected virtual void Body(Document doc, PdfWriter writer, PdfDocument pdfDoc)
    {
        // Override to implement body content
    }
    
    protected virtual void Header(Document doc, PdfWriter writer, PdfDocument pdfDoc)
    {
        // Override to implement header content
    }
    
    protected virtual void Footer(Document doc, PdfWriter writer, PdfDocument pdfDoc)
    {
        // Override to implement footer content
    }

    //...
}
```

### BaseSpreadsheet Architecture

The BaseSpreadsheet class provides a foundation for all Excel spreadsheets in the application. It handles Excel generation using ClosedXML with support for formatting and styling.

**Key Features:**

- **Generate()**: Main method for Excel file generation
- **Spreadsheed()**: Abstract method for spreadsheet content implementation
- **ExcelTable**: Helper class for table creation with styling support

```csharp
public abstract class BaseSpreadsheet
{
    public string Generate()
    {
        using (var workbook = new XLWorkbook())
        {
            Spreadsheed(workbook);
            /...
        }
    }
    
    public abstract void Spreadsheed(XLWorkbook workbook);
}
```

```csharp
// ExcelTable helper for easy table creation
public class ExcelTable
{
    public ExcelTable Cell(string text, Action<IXLStyle> style = null)
    {
        //...
    }
}
```

### Razor Templates

Razor templates are used for HTML content rendering, providing a flexible and maintainable way to create report layouts. The templates support CSS styling and parameter injection.

**Template Structure:**

```razor
@* RenderMessage.razor *@
<div class="custom-message">
    <h1 class="title">@Title</h1>
    <p class="text">@Message</p>
</div>

@code {
    [Parameter] public string Message { get; set; }
    [Parameter] public string Title { get; set; }
}
```

**Rendering Process:**

```csharp
var html = await Render<RenderMessage>(new Dictionary<string, object?>
{
    { "Message", "Conteúdo da mensagem" },
    { "Title", "Título do relatório" }
});
```

### API Endpoints

The application provides REST API endpoints for report generation and health monitoring.

**Available Endpoints:**

```http
GET /reports/ping
```
Returns OK status to verify API functionality.

```http
GET /reports/test
```
Generates a sample PDF report and returns the file for download.

**Usage Example:**

```csharp
[HttpGet("test")]
public IActionResult Test()
{
    var base64 = _report.Generate().Base64;
    var fileBytes = Convert.FromBase64String(base64);
    var contentType = "application/pdf";
    var fileName = "report.pdf";

    return File(fileBytes, contentType, fileName);
}
```

### Configuration

The configuration system allows customization of report parameters, margins, and page settings.

**Margins Configuration:**

```csharp
var margins = new Margins(40, 40, 40, 40); // Top, Right, Bottom, Left
```

**Page Size and Layout:**

```csharp
Build(margins, headerHeight: 150, footerHeight: 150, pageSize: PageSize.A4);
```

**Dependency Injection:**

```csharp
services.AddScoped<RenderMessageReport>();
```

## Development Guide

### Creating New Reports


To create new reports, see the advanced examples below:

---

### Spreadsheet Example (BaseSpreadsheet)

```csharp
public class SalesSpreadsheet : BaseSpreadsheet
{
    public override void Spreadsheed(XLWorkbook workbook)
    {
        var ws = workbook.Worksheets.Add("Sales Data");
        var table = new ExcelTable(4, 1, 1, ws);

        // Header row
        table.Cell("Product", style => { style.Font.Bold = true; style.Fill.BackgroundColor = XLColor.DarkBlue; style.Font.FontColor = XLColor.White; })
             .Cell("Quantity", style => { style.Font.Bold = true; style.Fill.BackgroundColor = XLColor.DarkBlue; style.Font.FontColor = XLColor.White; })
             .Cell("Unit Price", style => { style.Font.Bold = true; style.Fill.BackgroundColor = XLColor.DarkBlue; style.Font.FontColor = XLColor.White; })
             .Cell("Total", style => { style.Font.Bold = true; style.Fill.BackgroundColor = XLColor.DarkBlue; style.Font.FontColor = XLColor.White; });

        // Data rows
        table.Cell("Laptop").Cell("5").Cell("$1200").Cell("$6000");
        table.Cell("Mouse").Cell("20").Cell("$25").Cell("$500");
        table.Cell("Keyboard").Cell("10").Cell("$80").Cell("$800");
    }
}

```

### Report Example 1: Using Razor/HTML (BaseReport)

```csharp
public class InvoiceReport : BaseReport
{
    public InvoiceReport(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public InvoiceReport Generate(string customer, decimal total)
    {
        Build(new Margins(40, 40, 40, 40), 120, 80);
        _customer = customer;
        _total = total;
        return this;
    }

    private string _customer;
    private decimal _total;

    protected override void Body(Document doc, PdfWriter writer, PdfDocument pdfDoc)
    {
        var html = Render<InvoiceTemplate>(new Dictionary<string, object?>
        {
            { "Customer", _customer },
            { "Total", _total }
        }).Result;
        doc.AddHtml(html);
    }
}
```

**InvoiceTemplate.razor**
```razor
<div class="invoice">
    <h2>Invoice for @Customer</h2>
    <p>Total Amount: <b>@Total.ToString("C")</b></p>
</div>

@code {
    [Parameter] public string Customer { get; set; }
    [Parameter] public decimal Total { get; set; }
}
```

### Report Example 2: Using iText Methods (BaseReport)

```csharp
public class SimpleTextReport : BaseReport
{
    public SimpleTextReport(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public SimpleTextReport Generate(string title, string message)
    {
        Build(new Margins(30, 30, 30, 30), 80, 80);
        _title = title;
        _message = message;
        return this;
    }

    private string _title;
    private string _message;

    protected override void Body(Document doc, PdfWriter writer, PdfDocument pdfDoc)
    {
        var titlePara = new Paragraph(_title)
            .SetFontSize(18)
            .SetBold()
            .SetTextAlignment(TextAlignment.CENTER);
        var messagePara = new Paragraph(_message)
            .SetFontSize(12)
            .SetTextAlignment(TextAlignment.LEFT);

        doc.Add(titlePara);
        doc.Add(messagePara);
    }
}
```

### Creating New Spreadsheets

To create a new Excel spreadsheet, follow these steps:

**1. Create Spreadsheet Class:**

```csharp
public class MinhaPlanilha : BaseSpreadsheet
{
    public override void Spreadsheed(XLWorkbook workbook)
    {
        var worksheet = workbook.Worksheets.Add("Dados");
        
        // Create table with styling
        var table = new ExcelTable(3, 1, 1, worksheet);
        table.Cell("Nome", style => style.Font.Bold = true)
             .Cell("Idade", style => style.Font.Bold = true)
             .Cell("Email", style => style.Font.Bold = true)
             .Cell("João", style => style.Fill.BackgroundColor = XLColor.LightGray)
             .Cell("25")
             .Cell("joao@email.com");
    }
}
```

### Customization

**CSS Styling:**

The Razor templates support inline CSS for customization:

```css
.custom-message {
    flex: 1;
    text-align: center;
    word-break: break-word; 
    text-align: justify;
}

.box {
    display: flex;
    flex-direction: column;
    height: fit-content;
    width: 100%;
    border-radius: 8px;
}
```

**Excel Styling:**

Advanced Excel formatting with ClosedXML:

```csharp
table.Cell("Header", style => 
{
    style.Font.Bold = true;
    style.Font.FontSize = 14;
    style.Fill.BackgroundColor = XLColor.Blue;
    style.Font.FontColor = XLColor.White;
});
```

**Event Handlers:**

Custom event handlers for header and footer management:

- **HeaderEventHandler**: Manages header on each page
- **FooterEventHandler**: Manages footer on each page

### Debugging

**Logging Configuration:**

Configure logging in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Deployment

### Production Configuration

1. Update connection strings for production database
2. Configure proper JWT/security keys if needed
3. Set up HTTPS certificates
4. Configure logging for production environment
5. Set up proper CORS policies

### Docker Support

The project can be containerized using Docker:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
COPY ["Infra.Reports/Infra.Reports.csproj", "Infra.Reports/"]
COPY ["Infra.Reports.Razor/Infra.Reports.Razor.csproj", "Infra.Reports.Razor/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
```

### License

This project is licensed under the MIT License - see the LICENSE file for details.