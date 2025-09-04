# Splunk Log Analyzer - Software Requirement Document

## 1. Project Overview

**Project Name:** Splunk Log Analyzer  
**Version:** 1.0  
**Document Type:** Software Requirement Specification (SRS)  
**Date:** September 4, 2025

## 2. Business Requirements

### 2.1 Purpose
Develop a WPF desktop application to automate the process of searching and extracting transaction information from Splunk logs based on specific codes and time ranges.

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
- **FR-005:** The system SHALL display imported codes in a data grid with status indicators
- **FR-006:** The system SHALL allow manual addition/removal of individual codes

#### 3.2.2 Search Execution
- **FR-007:** The system SHALL allow users to specify time range for log searches
- **FR-008:** The system SHALL execute searches for all imported codes simultaneously or sequentially
- **FR-009:** The system SHALL display real-time progress during search operations

#### 3.2.3 Results Management
- **FR-010:** The system SHALL display search results in a structured table format showing:
- Original code
- `issuerBankName`
- `remitterName`
- `remitterAccountNumber`
- Search status (Success/Failed/No Data)
- **FR-011:** The system SHALL allow export of results to CSV/Excel formats

#### 3.2.4 Settings Configuration
- **FR-012:** The system SHALL provide a settings panel to configure:
- Splunk server connection parameters (URL, credentials)
- Default time range settings
- Export file preferences
- API timeout settings

## 4. Non-Functional Requirements

### 4.1 User Interface
- **NFR-001:** The application SHALL be built using WPF with Fluent Design principles
- **NFR-002:** The application SHALL use WPF-UI library (https://www.nuget.org/packages/WPF-UI/)
- **NFR-003:** The interface SHALL be responsive and intuitive for business users

### 4.2 Performance
- **NFR-004:** The system SHALL handle up to 1000 codes in a single batch operation
- **NFR-005:** API calls SHALL have configurable timeout settings (default: 30 seconds)

### 4.3 Integration
- **NFR-006:** The system SHALL integrate with Splunk REST API
- **NFR-007:** The system SHALL handle API authentication and session management

### 4.4 Error Handling
- **NFR-008:** The system SHALL provide clear error messages for failed operations
- **NFR-009:** The system SHALL log all operations for troubleshooting purposes

## 5. Technical Specifications

### 5.1 Technology Stack
- **Platform:** .NET 8
- **UI Framework:** WPF with WPF-UI library
- **API Integration:** Splunk REST API
- **Data Formats:** JSON parsing, CSV/Excel export

### 5.2 Data Model
```json
{
"searchCode": "string",
"issuerBankName": "string",
"remitterName": "string", 
"remitterAccountNumber": "string",
"searchStatus": "enum",
"timestamp": "datetime"
}
```

## 6. User Stories

### 6.1 As a Business Analyst
- I want to import a list of transaction codes so that I can search for multiple transactions at once
- I want to export search results so that I can analyze data in Excel
- I want to configure Splunk connection settings so that I can connect to different environments

### 6.2 As an Operations User  
- I want to see real-time progress during searches so that I know the system is working
- I want clear error messages when searches fail so that I can troubleshoot issues
- I want to specify custom time ranges so that I can search within specific periods

## 7. Acceptance Criteria

### 7.1 Core Functionality
- ✅ Successfully connect to Splunk API with valid credentials
- ✅ Import minimum 100 codes from CSV file without errors
- ✅ Extract all three required fields (issuerBankName, remitterName, remitterAccountNumber) when data exists
- ✅ Export results to CSV format with all columns properly formatted

### 7.2 User Experience
- ✅ Application loads within 5 seconds
- ✅ Search progress is visible to user
- ✅ All UI elements follow Fluent Design guidelines
- ✅ Settings are persisted between application sessions

## 8. Assumptions and Constraints

### 8.1 Assumptions
- Splunk API access and credentials will be provided
- Log format and JSON structure remain consistent
- Users have basic knowledge of transaction codes

### 8.2 Constraints
- Application must run on Windows 10/11
- Requires network connectivity to Splunk server
- Limited to Splunk REST API capabilities

## 9. Glossary

- **Code:** Transaction identifier used for Splunk log searches
- **transactionEntityAttribute:** JSON object containing transaction details in Splunk logs
- **Fluent Design:** Microsoft's design language for modern applications
- **WPF-UI:** Third-party UI library providing modern controls for WPF applications