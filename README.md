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
public class BaseReport
{
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
}
```

### BaseSpreadsheet Architecture

The BaseSpreadsheet class provides a foundation for all Excel spreadsheets in the application. It handles Excel generation using ClosedXML with support for advanced formatting and styling.

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
            // File generation logic
        }
    }
    
    public abstract void Spreadsheed(XLWorkbook workbook);
}

// ExcelTable helper for easy table creation
public class ExcelTable
{
    public ExcelTable Cell(string text, Action<IXLStyle> style = null)
    {
        // Cell creation with optional styling
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

To create a new PDF report, follow these steps:

**1. Create Report Class:**

```csharp
public class MeuRelatorio : BaseReport
{
    public MeuRelatorio(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public MeuRelatorio Generate()
    {
        Build(new Margins(40, 40, 40, 40), 150, 150);
        return this;
    }

    protected override void Body(Document doc, PdfWriter writer, PdfDocument pdfDoc)
    {
        var html = Render<MeuRelatorioTemplate>(new Dictionary<string, object?>
        {
            { "Content", "Conteúdo personalizado" }
        }).Result;
        
        doc.AddHtml(html);
    }
}
```

**2. Register in DI Container:**

```csharp
services.AddScoped<MeuRelatorio>();
```

**3. Create Razor Template (Optional):**

```razor
@* MeuRelatorioTemplate.razor *@
<div class="meu-relatorio">
    <h1>@Title</h1>
    <p>@Content</p>
</div>

@code {
    [Parameter] public string Title { get; set; }
    [Parameter] public string Content { get; set; }
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

**2. Usage:**

```csharp
var spreadsheet = new MinhaPlanilha();
var base64 = spreadsheet.Generate();
var fileBytes = Convert.FromBase64String(base64);

return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "planilha.xlsx");
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

**Swagger Documentation:**

Access https://localhost:7001/swagger to test endpoints directly and view API documentation.

**Common Issues:**

1. **PDF Generation Errors**: Check iText7 dependencies and license
2. **Excel Generation Errors**: Verify ClosedXML and EPPlus dependencies
3. **Razor Rendering Issues**: Verify component parameters and HTML structure
4. **Memory Issues**: Ensure proper disposal of PDF and Excel streams

## Technologies Used

- **.NET 8.0** - Main framework
- **iText7** - PDF generation library
- **ClosedXML** - Excel manipulation library
- **EPPlus** - Advanced Excel features
- **Razor Components** - HTML templating
- **ASP.NET Core Web API** - REST API framework
- **Swagger** - API documentation
- **Dependency Injection** - Service management

## Package Dependencies

### Api
- `itext.bouncy-castle-adapter` (8.0.4)
- `Microsoft.AspNetCore.Components.Web` (8.0.7)
- `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` (8.0.7)
- `Swashbuckle.AspNetCore` (6.6.2)

### Infra.Reports
- `itext7` (9.2.0)
- `itext7.pdfhtml` (6.2.0)
- `ClosedXML` (0.105.0)
- `EPPlus` (8.0.8)
- `Microsoft.AspNetCore.Components.Web` (8.0.7)

### Infra.Reports.Razor
- `Microsoft.AspNetCore.Components.Web` (8.0.7)
- `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation` (8.0.7)

## Support

If you encounter any issues or have questions:

1. Check iText7 documentation
2. Review ClosedXML and EPPlus documentation
3. Review application logs
4. Test endpoints via Swagger
5. Open an issue in the repository

## Useful Links

- [iText7 Documentation](https://itextpdf.com/en/products/itext-7)
- [ClosedXML Documentation](https://github.com/ClosedXML/ClosedXML)
- [EPPlus Documentation](https://epplussoftware.com/)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Razor Components](https://docs.microsoft.com/en-us/aspnet/core/blazor/components/)
- [Swagger Documentation](https://swagger.io/docs/) 