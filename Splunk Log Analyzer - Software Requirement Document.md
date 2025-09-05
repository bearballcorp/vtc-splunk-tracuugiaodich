# Splunk Log Analyzer - Software Requirement Document

## 1. Project Overview

**Project Name:** Splunk Log Analyzer  
**Version:** 1.0  
**Document Type:** Software Requirement Specification (SRS)  
**Date:** September 4, 2025

## 2. Business Requirements

### 2.1 Purpose
Develop a Windows Forms desktop application to automate the process of searching and extracting transaction information from Splunk logs based on specific codes and time ranges.

### 2.2 Scope
The application will integrate with Splunk API to search logs, filter transaction data, and export results in a structured format.

## 3. Functional Requirements

### 3.1 Core Business Logic

#### 3.1.1 Splunk Log Search
- **FR-001:** The system SHALL search Splunk logs using the query pattern: `{code} source="bankgatev2-public"`
- Where `{code}` is a user-provided search parameter
- Within a specified time range (start date/time to end date/time)

#### 3.1.2 Log Filtering and Data Extraction
- **FR-002:** The system SHALL identify the first log entry containing `transactionEntityAttribute` JSON object
- **FR-003:** The system SHALL extract the following fields from the JSON:
- `issuerBankName`
- `remitterName` 
- `remitterAccountNumber`

### 3.2 User Interface Features

#### 3.2.1 Code Management
- **FR-004:** The system SHALL allow users to import a list of codes from external files (CSV, Excel, TXT)
- **FR-005:** The system SHALL display imported codes in a DataGridView with status indicators
- **FR-006:** The system SHALL allow manual addition/removal of individual codes through form controls

#### 3.2.2 Search Execution
- **FR-007:** The system SHALL allow users to specify time range using DateTimePicker controls
- **FR-008:** The system SHALL execute searches for all imported codes with progress indication
- **FR-009:** The system SHALL display real-time progress using ProgressBar and status labels

#### 3.2.3 Results Management
- **FR-010:** The system SHALL display search results in a DataGridView showing:
- Original code
- `issuerBankName`
- `remitterName`
- `remitterAccountNumber`
- Search status (Success/Failed/No Data)
- **FR-011:** The system SHALL allow export of results to CSV/Excel formats using SaveFileDialog

#### 3.2.4 Settings Configuration
- **FR-012:** The system SHALL provide a settings form to configure:
- Splunk server connection parameters (URL, credentials)
- Default time range settings
- Export file preferences
- API timeout settings

## 4. Non-Functional Requirements

### 4.1 User Interface
- **NFR-001:** The application SHALL be built using Windows Forms (.NET Framework or .NET Core)
- **NFR-002:** The interface SHALL follow Windows UI guidelines for consistency
- **NFR-003:** The application SHALL use modern flat design with clean layouts
- **NFR-004:** All forms SHALL be resizable with proper anchor/dock settings

### 4.2 Performance
- **NFR-005:** The system SHALL handle up to 1000 codes in a single batch operation
- **NFR-006:** API calls SHALL have configurable timeout settings (default: 30 seconds)
- **NFR-007:** UI SHALL remain responsive during long-running operations using BackgroundWorker

### 4.3 Integration
- **NFR-008:** The system SHALL integrate with Splunk REST API
- **NFR-009:** The system SHALL handle API authentication and session management

### 4.4 Error Handling
- **NFR-010:** The system SHALL provide clear error messages using MessageBox dialogs
- **NFR-011:** The system SHALL log all operations for troubleshooting purposes

## 5. Technical Specifications

### 5.1 Technology Stack
- **Platform:** .NET 8
- **UI Framework:** Windows Forms
- **API Integration:** Splunk REST API using HttpClient
- **Data Formats:** JSON parsing (Newtonsoft.Json), CSV/Excel export
- **Threading:** BackgroundWorker for async operations

### 5.2 Form Structure
```
MainForm
├── MenuStrip (File, Settings, Help)
├── ToolStrip (Import, Export, Search buttons)
├── GroupBox: Search Parameters
│   ├── DateTimePicker: Start Time
│   ├── DateTimePicker: End Time
│   └── Button: Search
├── GroupBox: Code Management
│   ├── DataGridView: Code List
│   ├── TextBox: Manual Code Entry
│   └── Buttons: Add, Remove, Import
├── GroupBox: Results
│   ├── DataGridView: Search Results
│   └── ProgressBar: Search Progress
└── StatusStrip: Status information

SettingsForm
├── GroupBox: Splunk Connection
│   ├── TextBox: Server URL
│   ├── TextBox: Username
│   ├── TextBox: Password (masked)
│   └── NumericUpDown: Timeout
└── Buttons: OK, Cancel, Test Connection
```

### 5.3 Data Model
```csharp
public class SearchResult
{
  public string SearchCode { get; set; }
  public string IssuerBankName { get; set; }
  public string RemitterName { get; set; }
  public string RemitterAccountNumber { get; set; }
  public SearchStatus Status { get; set; }
  public DateTime Timestamp { get; set; }
}

public enum SearchStatus
{
  Pending,
  Success,
  Failed,
  NoData
}
```

## 6. User Stories

### 6.1 As a Business Analyst
- I want to import a list of transaction codes using a simple file dialog so that I can search for multiple transactions at once
- I want to export search results using standard Windows dialogs so that I can analyze data in Excel
- I want to configure Splunk connection through a dedicated settings form so that I can connect to different environments

### 6.2 As an Operations User  
- I want to see a progress bar during searches so that I know the system is working
- I want clear error message boxes when searches fail so that I can troubleshoot issues
- I want to specify custom time ranges using date/time pickers so that I can search within specific periods

## 7. Acceptance Criteria

### 7.1 Core Functionality
- ✅ Successfully connect to Splunk API with valid credentials
- ✅ Import minimum 100 codes from CSV file without errors
- ✅ Extract all three required fields (issuerBankName, remitterName, remitterAccountNumber) when data exists
- ✅ Export results to CSV format with all columns properly formatted

### 7.2 User Experience
- ✅ Application loads within 3 seconds
- ✅ Search progress is visible via progress bar and status updates
- ✅ All forms are properly resizable and follow Windows UI standards
- ✅ Settings are persisted using app.config or registry

### 7.3 Technical Requirements
- ✅ UI remains responsive during long operations using BackgroundWorker
- ✅ Proper error handling with user-friendly message boxes
- ✅ All DataGridView operations work smoothly (sorting, selection, etc.)

## 8. Implementation Notes

### 8.1 Key WinForms Components
- **DataGridView:** For displaying code lists and results with built-in sorting/filtering
- **BackgroundWorker:** For async API calls without freezing UI
- **ProgressBar + Timer:** For progress indication and status updates
- **OpenFileDialog/SaveFileDialog:** For file import/export operations
- **DateTimePicker:** For intuitive date/time selection

### 8.2 Architecture Simplification
- **No MVVM pattern:** Direct event-driven programming with form code-behind
- **Simple data binding:** Direct DataGridView.DataSource assignment
- **Straightforward navigation:** Modal dialogs for settings and confirmations

## 9. Assumptions and Constraints

### 9.1 Assumptions
- Splunk API access and credentials will be provided
- Log format and JSON structure remain consistent
- Users have basic knowledge of transaction codes
- Windows environment with .NET runtime available

### 9.2 Constraints
- Application must run on Windows 7/10/11
- Requires network connectivity to Splunk server
- Limited to Splunk REST API capabilities
- Single-threaded UI with background processing

## 10. Glossary

- **Code:** Transaction identifier used for Splunk log searches
- **transactionEntityAttribute:** JSON object containing transaction details in Splunk logs
- **Windows Forms:** Microsoft's GUI framework for desktop applications
- **BackgroundWorker:** .NET component for handling background operations
- **DataGridView:** Windows Forms control for displaying tabular data