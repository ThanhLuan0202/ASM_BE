# Third-Party Libraries - Audit Management System

## 📦 Tổng quan

Tài liệu này liệt kê tất cả các third-party libraries được sử dụng trong hệ thống, bao gồm cả Frontend và Backend.

---

## 🎨 FRONTEND (React Application)

### Core Framework & Language
| Package | Version | Mục đích |
|---------|---------|----------|
| `react` | ^19.1.1 | React framework - UI library |
| `react-dom` | ^19.1.1 | React DOM rendering |
| `typescript` | ~5.9.3 | TypeScript language support |

### Build Tools & Development
| Package | Version | Mục đích |
|---------|---------|----------|
| `vite` | ^7.1.7 | Build tool và dev server |
| `@vitejs/plugin-react` | ^5.0.4 | Vite plugin cho React |
| `@rollup/rollup-win32-x64-msvc` | ^4.52.4 | Rollup bundler (Windows) |

### Routing & Navigation
| Package | Version | Mục đích |
|---------|---------|----------|
| `react-router-dom` | ^7.9.4 | Client-side routing |

### State Management
| Package | Version | Mục đích |
|---------|---------|----------|
| `zustand` | ^5.0.8 | Lightweight state management (Client state) |
| `@tanstack/react-query` | ^5.90.6 | Server state management, caching, data fetching |

### HTTP Client & API Communication
| Package | Version | Mục đích |
|---------|---------|----------|
| `axios` | ^1.12.2 | HTTP client cho API calls |
| `@microsoft/signalr` | ^10.0.0 | SignalR client cho real-time communication |

### Authentication (JWT)
| Package | Version | Mục đích |
|---------|---------|----------|
| **Không có JWT library** | - | Frontend decode JWT thủ công bằng `atob()` (browser built-in) để lấy payload. Token được lưu trong Zustand store và gửi trong `Authorization: Bearer <token>` header |

### UI Components & Styling
| Package | Version | Mục đích |
|---------|---------|----------|
| `tailwindcss` | ^3.4.18 | Utility-first CSS framework |
| `autoprefixer` | ^10.4.21 | CSS autoprefixer |
| `postcss` | ^8.5.6 | CSS post-processor |
| `react-icons` | ^5.5.0 | Icon library |
| `react-toastify` | ^11.0.5 | Toast notification component |

### Data Visualization
| Package | Version | Mục đích |
|---------|---------|----------|
| `recharts` | ^3.3.0 | Chart library (React charts) |

### Animations & Interactions
| Package | Version | Mục đích |
|---------|---------|----------|
| `gsap` | ^3.13.0 | Animation library |
| `@gsap/react` | ^2.1.2 | GSAP React integration |
| `@dnd-kit/core` | ^6.3.1 | Drag and drop core |
| `@dnd-kit/sortable` | ^10.0.0 | Sortable drag and drop |
| `@dnd-kit/utilities` | ^3.2.2 | DnD Kit utilities |

### Code Quality & Linting
| Package | Version | Mục đích |
|---------|---------|----------|
| `eslint` | ^9.36.0 | JavaScript/TypeScript linter |
| `@eslint/js` | ^9.36.0 | ESLint JavaScript configuration |
| `eslint-plugin-react-hooks` | ^5.2.0 | ESLint plugin cho React hooks |
| `eslint-plugin-react-refresh` | ^0.4.22 | ESLint plugin cho React refresh |
| `typescript-eslint` | ^8.45.0 | TypeScript ESLint integration |
| `globals` | ^16.4.0 | Global variables cho ESLint |

### Type Definitions
| Package | Version | Mục đích |
|---------|---------|----------|
| `@types/node` | ^24.6.0 | TypeScript types cho Node.js |
| `@types/react` | ^19.1.16 | TypeScript types cho React |
| `@types/react-dom` | ^19.1.9 | TypeScript types cho React DOM |

---

## 🔧 BACKEND (ASP.NET Core)

### Core Framework
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `.NET 8.0` | 8.0 | .NET runtime và framework | All |

### Web Framework
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.5 | JWT Bearer authentication | ASM.API |
| `Microsoft.AspNetCore.Http.Abstractions` | 2.3.0 | HTTP abstractions | ASM_Services, ASM_Repositories |
| `Microsoft.AspNetCore.Http.Features` | 5.0.17 | HTTP features | ASM_Repositories |

### Database & ORM
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `Microsoft.EntityFrameworkCore` | 8.0.5 | Entity Framework Core ORM | ASM_Repositories |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.5 | SQL Server provider cho EF Core | ASM_Repositories |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.5 | EF Core design-time tools | ASM.API, ASM_Repositories |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.5 | EF Core command-line tools | ASM_Repositories |

### Authentication & Security
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `Microsoft.IdentityModel.Tokens` | 8.10.0 | Identity model tokens | ASM.API |
| `System.IdentityModel.Tokens.Jwt` | 8.10.0 | JWT token handling | ASM.API |
| `BCrypt.Net-Next` | 4.0.3 | Password hashing | ASM_Repositories |
| `Google.Apis.Auth` | 1.69.0 | Google API authentication | ASM.API |

### Object Mapping
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `AutoMapper` | 12.0.1 | Object-to-object mapping | ASM_Repositories |
| `AutoMapper.Extensions.Microsoft.DependencyInjection` | 12.0.1 | AutoMapper DI integration | ASM.API |

### Configuration & Dependency Injection
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `Microsoft.Extensions.Configuration` | 8.0.0 | Configuration system | ASM_Repositories |
| `Microsoft.Extensions.Configuration.Json` | 8.0.0 | JSON configuration provider | ASM_Repositories |
| `Microsoft.Extensions.Configuration.Abstractions` | 8.0.0 | Configuration abstractions | ASM_Services |
| `Microsoft.Extensions.DependencyInjection` | 8.0.1 | Dependency injection | ASM_Repositories |
| `Microsoft.Extensions.Options` | 8.0.0 | Options pattern | ASM_Services |

### File Storage & Cloud Services
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `FirebaseStorage.net` | 1.0.3 | Firebase Storage client | ASM_Repositories, ASM_Services |

### Email Service
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `MailKit` | 4.12.1 | Email client library | ASM_Repositories, ASM_Services |

### Document Generation
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `QuestPDF` | 2025.7.4 | PDF generation library | ASM.API, ASM_Services |
| `EPPlus` | 8.2.1 | Excel file generation | ASM.API |
| `System.Drawing.Common` | 10.0.0 | Graphics and image processing | ASM.API, ASM_Services |

### API Documentation
| Package | Version | Mục đích | Project |
|---------|---------|----------|---------|
| `Swashbuckle.AspNetCore` | 6.6.2 | Swagger/OpenAPI documentation | ASM.API |

---

## 📊 Tổng hợp theo mục đích

### Frontend - Tổng cộng: **27 packages**

#### Core & Framework (3)
- react, react-dom, typescript

#### Build & Dev Tools (5)
- vite, @vitejs/plugin-react, @rollup/rollup-win32-x64-msvc, autoprefixer, postcss

#### Routing & Navigation (1)
- react-router-dom

#### State Management (2)
- zustand, @tanstack/react-query

#### HTTP & Communication (2)
- axios, @microsoft/signalr

#### Authentication (JWT) (0)
- **Không có package**: Frontend decode JWT thủ công bằng `atob()` để lấy userId, deptId từ payload

#### UI & Styling (3)
- tailwindcss, react-icons, react-toastify

#### Data Visualization (1)
- recharts

#### Animations (5)
- gsap, @gsap/react, @dnd-kit/core, @dnd-kit/sortable, @dnd-kit/utilities

#### Code Quality (6)
- eslint, @eslint/js, eslint-plugin-react-hooks, eslint-plugin-react-refresh, typescript-eslint, globals

#### Type Definitions (3)
- @types/node, @types/react, @types/react-dom

---

### Backend - Tổng cộng: **23 packages**

#### Core Framework (1)
- .NET 8.0

#### Web Framework (3)
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.AspNetCore.Http.Abstractions
- Microsoft.AspNetCore.Http.Features

#### Database & ORM (4)
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.Tools

#### Authentication & Security (4)
- Microsoft.IdentityModel.Tokens
- System.IdentityModel.Tokens.Jwt
- BCrypt.Net-Next
- Google.Apis.Auth

#### Object Mapping (2)
- AutoMapper
- AutoMapper.Extensions.Microsoft.DependencyInjection

#### Configuration & DI (5)
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.Configuration.Abstractions
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Options

#### File Storage (1)
- FirebaseStorage.net

#### Email (1)
- MailKit

#### Document Generation (3)
- QuestPDF
- EPPlus
- System.Drawing.Common

#### API Documentation (1)
- Swashbuckle.AspNetCore

---

## 🔍 Chi tiết theo Project

### ASM.API (Presentation Layer)
- EPPlus
- QuestPDF
- Swashbuckle.AspNetCore
- AutoMapper.Extensions.Microsoft.DependencyInjection
- Google.Apis.Auth
- Microsoft.AspNetCore.Authentication.JwtBearer
- Microsoft.EntityFrameworkCore.Design
- Microsoft.IdentityModel.Tokens
- System.Drawing.Common
- System.IdentityModel.Tokens.Jwt

### ASM_Services (Business Logic Layer)
- FirebaseStorage.net
- Microsoft.AspNetCore.Http.Abstractions
- Microsoft.Extensions.Configuration.Abstractions
- Microsoft.Extensions.Options
- QuestPDF
- System.Drawing.Common
- MailKit

### ASM_Repositories (Data Access Layer)
- AutoMapper
- BCrypt.Net-Next
- FirebaseStorage.net
- MailKit
- Microsoft.AspNetCore.Http.Abstractions
- Microsoft.AspNetCore.Http.Features
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.Extensions.Configuration
- Microsoft.Extensions.Configuration.Json
- Microsoft.Extensions.DependencyInjection

---

## 📝 Ghi chú

### Frontend
- Tất cả packages được quản lý qua `npm`/`package.json`
- Dev dependencies chỉ dùng trong development
- Optional dependencies có thể không cần thiết trên mọi platform

### Backend
- Tất cả packages được quản lý qua NuGet
- Một số packages được reference gián tiếp qua ProjectReference
- EF Core Tools chỉ cần trong development

### Security Notes
- JWT packages: Authentication & Authorization
- BCrypt: Password hashing (one-way)
- Google.Apis.Auth: Google service authentication (nếu có)

### External Services
- **Firebase Storage**: File storage service
- **SMTP Server**: Email service (cấu hình trong appsettings.json)

---

## 🔄 Dependencies Tree

### Frontend
```
React 19
├── react-router-dom (Routing)
├── zustand (State)
├── @tanstack/react-query (Server State)
├── axios (HTTP)
├── @microsoft/signalr (Real-time)
└── tailwindcss (Styling)
```

### Backend
```
ASP.NET Core 8.0
├── Entity Framework Core (Database)
├── JWT Bearer (Authentication)
├── AutoMapper (Mapping)
├── FirebaseStorage.net (File Storage)
├── MailKit (Email)
├── QuestPDF (PDF)
└── EPPlus (Excel)
```

---

*Tài liệu này được tạo tự động dựa trên package.json và .csproj files.*

**Last Updated:** 2024

