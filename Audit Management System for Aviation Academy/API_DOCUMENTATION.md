# DANH SÁCH API - AUDIT MANAGEMENT SYSTEM FOR AVIATION ACADEMY

## 1. AUTHENTICATION & AUTHORIZATION

### AuthController (`/api/Auth`)

| Method | Endpoint                   | Mô tả                           |
| ------ | -------------------------- | ------------------------------- |
| POST   | `/api/Auth/login`          | Đăng nhập người dùng            |
| POST   | `/api/Auth/register`       | Đăng ký người dùng mới          |
| POST   | `/api/Auth/bulk-register`  | Đăng ký hàng loạt từ file Excel |
| POST   | `/api/Auth/reset-password` | Đặt lại mật khẩu                |

---

## 2. AUDIT MANAGEMENT

### AuditsController (`/api/Audits`)

| Method | Endpoint                                    | Mô tả                                              |
| ------ | ------------------------------------------- | -------------------------------------------------- |
| GET    | `/api/Audits`                               | Lấy danh sách tất cả audits                        |
| GET    | `/api/Audits/{id}`                          | Lấy thông tin audit theo ID                        |
| POST   | `/api/Audits`                               | Tạo audit mới                                      |
| PUT    | `/api/Audits/{id}`                          | Cập nhật audit                                     |
| DELETE | `/api/Audits/{id}`                          | Xóa audit (soft delete)                            |
| POST   | `/api/Audits/{id}/submit-to-lead-auditor`   | Gửi audit cho Lead Auditor                         |
| POST   | `/api/Audits/{id}/approve-forward-director` | Phê duyệt và chuyển cho Director                   |
| POST   | `/api/Audits/{id}/declined-plan-content`    | Từ chối nội dung kế hoạch                          |
| POST   | `/api/Audits/{id}/approve-plan`             | Phê duyệt kế hoạch                                 |
| POST   | `/api/Audits/{id}/reject-plan-content`      | Từ chối nội dung kế hoạch                          |
| GET    | `/api/Audits/Summary/{auditId}`             | Lấy tóm tắt audit                                  |
| POST   | `/api/Audits/Submit/{auditId}`              | Gửi audit (tạo PDF và upload)                      |
| PUT    | `/api/Audits/archive/{auditId}`             | Lưu trữ audit                                      |
| PUT    | `/api/Audits/{id}/complete-update`          | Cập nhật toàn bộ audit và các entities liên quan   |
| GET    | `/api/Audits/{auditId}/chart/line`          | Lấy dữ liệu biểu đồ line (findings theo tháng)     |
| GET    | `/api/Audits/{auditId}/chart/pie`           | Lấy dữ liệu biểu đồ pie (severity breakdown)       |
| GET    | `/api/Audits/{auditId}/chart/bar`           | Lấy dữ liệu biểu đồ bar (findings theo department) |
| GET    | `/api/Audits/ExportPdf/{auditId}`           | Xuất PDF audit report                              |
| GET    | `/api/Audits/ExportExcel/{auditId}`         | Xuất Excel audit report                            |
| GET    | `/api/Audits/by-period`                     | Lấy audits theo khoảng thời gian                   |
| POST   | `/api/Audits/validate-department`           | Validate department uniqueness                     |
| GET    | `/api/Audits/period-status`                 | Lấy trạng thái theo khoảng thời gian               |

### AuditPlanController (`/api/AuditPlan`)

| Method | Endpoint                   | Mô tả                     |
| ------ | -------------------------- | ------------------------- |
| GET    | `/api/AuditPlan`           | Lấy danh sách audit plans |
| GET    | `/api/AuditPlan/{auditId}` | Lấy chi tiết audit plan   |
| PUT    | `/api/AuditPlan/{auditId}` | Cập nhật audit plan       |

### AuditPlanAssignmentController (`/api/AuditPlanAssignment`)

| Method | Endpoint                             | Mô tả                                 |
| ------ | ------------------------------------ | ------------------------------------- |
| GET    | `/api/AuditPlanAssignment`           | Lấy danh sách tất cả assignments      |
| GET    | `/api/AuditPlanAssignment/{id}`      | Lấy assignment theo ID                |
| POST   | `/api/AuditPlanAssignment`           | Tạo assignment mới                    |
| PUT    | `/api/AuditPlanAssignment/{id}`      | Cập nhật assignment                   |
| DELETE | `/api/AuditPlanAssignment/{id}`      | Xóa assignment                        |
| GET    | `/api/AuditPlanAssignment/by-period` | Lấy assignments theo khoảng thời gian |
| POST   | `/api/AuditPlanAssignment/validate`  | Validate assignment                   |

---

## 3. AUDIT ASSIGNMENT

### AuditAssignmentController (`/api/AuditAssignment`)

| Method | Endpoint                                      | Mô tả                              |
| ------ | --------------------------------------------- | ---------------------------------- |
| GET    | `/api/AuditAssignment`                        | Lấy danh sách tất cả assignments   |
| GET    | `/api/AuditAssignment/{assignmentId}`         | Lấy assignment theo ID             |
| GET    | `/api/AuditAssignment/audit/{auditId}`        | Lấy assignments theo audit ID      |
| GET    | `/api/AuditAssignment/auditor/{auditorId}`    | Lấy assignments theo auditor ID    |
| GET    | `/api/AuditAssignment/department/{deptId}`    | Lấy assignments theo department ID |
| GET    | `/api/AuditAssignment/my-assignments`         | Lấy assignments của user hiện tại  |
| POST   | `/api/AuditAssignment`                        | Tạo assignment mới                 |
| PUT    | `/api/AuditAssignment/{assignmentId}`         | Cập nhật assignment                |
| DELETE | `/api/AuditAssignment/{assignmentId}`         | Xóa assignment                     |
| GET    | `/api/AuditAssignment/auditors-with-schedule` | Lấy danh sách auditors có schedule |

---

## 4. AUDIT TEAM

### AuditTeamController (`/api/AuditTeam`)

| Method | Endpoint                                      | Mô tả                                    |
| ------ | --------------------------------------------- | ---------------------------------------- |
| GET    | `/api/AuditTeam`                              | Lấy danh sách tất cả audit teams         |
| GET    | `/api/AuditTeam/{id}`                         | Lấy audit team theo ID                   |
| POST   | `/api/AuditTeam`                              | Tạo audit team mới                       |
| PUT    | `/api/AuditTeam/{id}`                         | Cập nhật audit team                      |
| DELETE | `/api/AuditTeam/{id}`                         | Xóa audit team (soft delete)             |
| GET    | `/api/AuditTeam/check-lead-auditor/{auditId}` | Kiểm tra user có phải Lead Auditor không |

---

## 5. AUDIT SCOPE & SCHEDULE

### AuditScopeDepartmentController (`/api/AuditScopeDepartment`)

| Method | Endpoint                         | Mô tả                                  |
| ------ | -------------------------------- | -------------------------------------- |
| GET    | `/api/AuditScopeDepartment`      | Lấy danh sách tất cả scope departments |
| GET    | `/api/AuditScopeDepartment/{id}` | Lấy scope department theo ID           |
| POST   | `/api/AuditScopeDepartment`      | Tạo scope department mới               |
| PUT    | `/api/AuditScopeDepartment/{id}` | Cập nhật scope department              |
| DELETE | `/api/AuditScopeDepartment/{id}` | Xóa scope department                   |

### AuditScheduleController (`/api/AuditSchedule`)

| Method | Endpoint                  | Mô tả                          |
| ------ | ------------------------- | ------------------------------ |
| GET    | `/api/AuditSchedule`      | Lấy danh sách tất cả schedules |
| GET    | `/api/AuditSchedule/{id}` | Lấy schedule theo ID           |
| POST   | `/api/AuditSchedule`      | Tạo schedule mới               |
| PUT    | `/api/AuditSchedule/{id}` | Cập nhật schedule              |
| DELETE | `/api/AuditSchedule/{id}` | Xóa schedule                   |

---

## 6. FINDINGS

### FindingsController (`/api/Findings`)

| Method | Endpoint                                    | Mô tả                          |
| ------ | ------------------------------------------- | ------------------------------ |
| GET    | `/api/Findings`                             | Lấy danh sách tất cả findings  |
| GET    | `/api/Findings/{id}`                        | Lấy finding theo ID            |
| POST   | `/api/Findings`                             | Tạo finding mới                |
| PUT    | `/api/Findings/{id}`                        | Cập nhật finding               |
| DELETE | `/api/Findings/{id}`                        | Xóa finding (soft delete)      |
| GET    | `/api/Findings/by-department/{deptId}`      | Lấy findings theo department   |
| GET    | `/api/Findings/by-audit-item/{auditItemId}` | Lấy findings theo audit item   |
| PUT    | `/api/Findings/{findingId}/received`        | Đánh dấu finding đã nhận       |
| GET    | `/api/Findings/by-audit/{auditId}`          | Lấy findings theo audit        |
| GET    | `/api/Findings/by-created-by/{createdBy}`   | Lấy findings theo người tạo    |
| GET    | `/api/Findings/my-findings`                 | Lấy findings của user hiện tại |

### FindingStatusController (`/api/FindingStatus`)

| Method | Endpoint                      | Mô tả                                 |
| ------ | ----------------------------- | ------------------------------------- |
| GET    | `/api/FindingStatus`          | Lấy danh sách tất cả finding statuses |
| GET    | `/api/FindingStatus/{status}` | Lấy finding status theo tên           |
| POST   | `/api/FindingStatus`          | Tạo finding status mới                |
| PUT    | `/api/FindingStatus/{status}` | Cập nhật finding status               |
| DELETE | `/api/FindingStatus/{status}` | Xóa finding status                    |

### FindingSeverityController (`/api/FindingSeverity`)

| Method | Endpoint                          | Mô tả                                   |
| ------ | --------------------------------- | --------------------------------------- |
| GET    | `/api/FindingSeverity`            | Lấy danh sách tất cả finding severities |
| GET    | `/api/FindingSeverity/{severity}` | Lấy finding severity theo tên           |
| POST   | `/api/FindingSeverity`            | Tạo finding severity mới                |
| PUT    | `/api/FindingSeverity/{severity}` | Cập nhật finding severity               |
| DELETE | `/api/FindingSeverity/{severity}` | Xóa finding severity                    |

---

## 7. ACTIONS

### ActionController (`/api/Action`)

| Method | Endpoint                                        | Mô tả                                  |
| ------ | ----------------------------------------------- | -------------------------------------- |
| GET    | `/api/Action`                                   | Lấy danh sách tất cả actions           |
| GET    | `/api/Action/{id}`                              | Lấy action theo ID                     |
| GET    | `/api/Action/my-assigned`                       | Lấy actions được gán cho user hiện tại |
| POST   | `/api/Action`                                   | Tạo action mới                         |
| PUT    | `/api/Action/{id}`                              | Cập nhật action                        |
| DELETE | `/api/Action/{id}`                              | Xóa action                             |
| POST   | `/api/Action/{id}/status/in-progress`           | Cập nhật status thành InProgress       |
| POST   | `/api/Action/{id}/status/reviewed`              | Cập nhật status thành Reviewed         |
| POST   | `/api/Action/{id}/status/approved`              | Cập nhật status thành Approved         |
| POST   | `/api/Action/{id}/status/rejected`              | Cập nhật status thành Rejected         |
| PUT    | `/api/Action/{id}/progress-percent`             | Cập nhật phần trăm tiến độ             |
| POST   | `/api/Action/{id}/status/closed`                | Cập nhật status thành Closed           |
| PUT    | `/api/Action/{id}/approve-by-auditor`           | Phê duyệt bởi auditor                  |
| GET    | `/api/Action/by-finding/{findingId}`            | Lấy actions theo finding               |
| GET    | `/api/Action/by-assigned-dept/{assignedDeptId}` | Lấy actions theo department            |
| PUT    | `/api/Action/{id}/reject`                       | Từ chối action                         |

### ActionReviewController (`/api/ActionReview`)

| Method | Endpoint                                | Mô tả                       |
| ------ | --------------------------------------- | --------------------------- |
| POST   | `/api/ActionReview/{actionId}/verified` | Xác nhận action đã verified |
| POST   | `/api/ActionReview/{actionId}/declined` | Từ chối action              |

---

## 8. CHECKLIST MANAGEMENT

### ChecklistTemplatesController (`/api/ChecklistTemplates`)

| Method | Endpoint                       | Mô tả                                    |
| ------ | ------------------------------ | ---------------------------------------- |
| GET    | `/api/ChecklistTemplates`      | Lấy danh sách tất cả checklist templates |
| GET    | `/api/ChecklistTemplates/{id}` | Lấy checklist template theo ID           |
| POST   | `/api/ChecklistTemplates`      | Tạo checklist template mới               |
| PUT    | `/api/ChecklistTemplates/{id}` | Cập nhật checklist template              |
| DELETE | `/api/ChecklistTemplates/{id}` | Xóa checklist template                   |

### ChecklistItemsController (`/api/ChecklistItems`)

| Method | Endpoint                   | Mô tả                                |
| ------ | -------------------------- | ------------------------------------ |
| GET    | `/api/ChecklistItems`      | Lấy danh sách tất cả checklist items |
| GET    | `/api/ChecklistItems/{id}` | Lấy checklist item theo ID           |
| POST   | `/api/ChecklistItems`      | Tạo checklist item mới               |
| PUT    | `/api/ChecklistItems/{id}` | Cập nhật checklist item              |
| DELETE | `/api/ChecklistItems/{id}` | Xóa checklist item                   |

### ChecklistItemNoFindingController (`/api/ChecklistItemNoFinding`)

| Method | Endpoint                           | Mô tả                            |
| ------ | ---------------------------------- | -------------------------------- |
| GET    | `/api/ChecklistItemNoFinding`      | Lấy danh sách tất cả no findings |
| GET    | `/api/ChecklistItemNoFinding/{id}` | Lấy no finding theo ID           |
| POST   | `/api/ChecklistItemNoFinding`      | Tạo no finding mới               |
| PUT    | `/api/ChecklistItemNoFinding/{id}` | Cập nhật no finding              |
| DELETE | `/api/ChecklistItemNoFinding/{id}` | Xóa no finding                   |

### AuditChecklistItemsController (`/api/AuditChecklistItems`)

| Method | Endpoint                        | Mô tả                                      |
| ------ | ------------------------------- | ------------------------------------------ |
| GET    | `/api/AuditChecklistItems`      | Lấy danh sách tất cả audit checklist items |
| GET    | `/api/AuditChecklistItems/{id}` | Lấy audit checklist item theo ID           |
| POST   | `/api/AuditChecklistItems`      | Tạo audit checklist item mới               |
| PUT    | `/api/AuditChecklistItems/{id}` | Cập nhật audit checklist item              |
| DELETE | `/api/AuditChecklistItems/{id}` | Xóa audit checklist item                   |

### AuditChecklistTemplateMapsController (`/api/AuditChecklistTemplateMaps`)

| Method | Endpoint                               | Mô tả                              |
| ------ | -------------------------------------- | ---------------------------------- |
| GET    | `/api/AuditChecklistTemplateMaps`      | Lấy danh sách tất cả template maps |
| GET    | `/api/AuditChecklistTemplateMaps/{id}` | Lấy template map theo ID           |
| POST   | `/api/AuditChecklistTemplateMaps`      | Tạo template map mới               |
| PUT    | `/api/AuditChecklistTemplateMaps/{id}` | Cập nhật template map              |
| DELETE | `/api/AuditChecklistTemplateMaps/{id}` | Xóa template map                   |

---

## 9. AUDIT CRITERIA

### AuditCriterionController (`/api/AuditCriterion`)

| Method | Endpoint                   | Mô tả                               |
| ------ | -------------------------- | ----------------------------------- |
| GET    | `/api/AuditCriterion`      | Lấy danh sách tất cả audit criteria |
| GET    | `/api/AuditCriterion/{id}` | Lấy audit criterion theo ID         |
| POST   | `/api/AuditCriterion`      | Tạo audit criterion mới             |
| PUT    | `/api/AuditCriterion/{id}` | Cập nhật audit criterion            |
| DELETE | `/api/AuditCriterion/{id}` | Xóa audit criterion                 |

### AuditCriteriaMapController (`/api/AuditCriteriaMap`)

| Method | Endpoint                     | Mô tả                              |
| ------ | ---------------------------- | ---------------------------------- |
| GET    | `/api/AuditCriteriaMap`      | Lấy danh sách tất cả criteria maps |
| GET    | `/api/AuditCriteriaMap/{id}` | Lấy criteria map theo ID           |
| POST   | `/api/AuditCriteriaMap`      | Tạo criteria map mới               |
| PUT    | `/api/AuditCriteriaMap/{id}` | Cập nhật criteria map              |
| DELETE | `/api/AuditCriteriaMap/{id}` | Xóa criteria map                   |

---

## 10. AUDIT APPROVAL & DOCUMENTS

### AuditApprovalController (`/api/AuditApproval`)

| Method | Endpoint                  | Mô tả                          |
| ------ | ------------------------- | ------------------------------ |
| GET    | `/api/AuditApproval`      | Lấy danh sách tất cả approvals |
| GET    | `/api/AuditApproval/{id}` | Lấy approval theo ID           |
| POST   | `/api/AuditApproval`      | Tạo approval mới               |
| PUT    | `/api/AuditApproval/{id}` | Cập nhật approval              |
| DELETE | `/api/AuditApproval/{id}` | Xóa approval                   |

### AuditDocumentsController (`/api/AuditDocuments`)

| Method | Endpoint                   | Mô tả                                |
| ------ | -------------------------- | ------------------------------------ |
| GET    | `/api/AuditDocuments`      | Lấy danh sách tất cả audit documents |
| GET    | `/api/AuditDocuments/{id}` | Lấy audit document theo ID           |

---

## 11. ROOT CAUSES

### RootCausesController (`/api/RootCauses`)

| Method | Endpoint               | Mô tả                            |
| ------ | ---------------------- | -------------------------------- |
| GET    | `/api/RootCauses`      | Lấy danh sách tất cả root causes |
| GET    | `/api/RootCauses/{id}` | Lấy root cause theo ID           |
| POST   | `/api/RootCauses`      | Tạo root cause mới               |
| PUT    | `/api/RootCauses/{id}` | Cập nhật root cause              |
| DELETE | `/api/RootCauses/{id}` | Xóa root cause                   |

---

## 12. ROLES

### RoleController (`/api/Role`)

| Method | Endpoint               | Mô tả                      |
| ------ | ---------------------- | -------------------------- |
| GET    | `/api/Role`            | Lấy danh sách tất cả roles |
| GET    | `/api/Role/{roleName}` | Lấy role theo tên          |
| POST   | `/api/Role`            | Tạo role mới               |
| PUT    | `/api/Role/{roleName}` | Cập nhật role              |
| DELETE | `/api/Role/{roleName}` | Xóa role                   |

---

## 13. REPORTS

### AuditReportsController (`/api/AuditReports`)

| Method | Endpoint            | Mô tả                              |
| ------ | ------------------- | ---------------------------------- |
| GET    | `/api/AuditReports` | Lấy danh sách tất cả audit reports |

### ReportRequestController (`/api/ReportRequest`)

| Method | Endpoint                  | Mô tả                                |
| ------ | ------------------------- | ------------------------------------ |
| GET    | `/api/ReportRequest`      | Lấy danh sách tất cả report requests |
| GET    | `/api/ReportRequest/{id}` | Lấy report request theo ID           |
| POST   | `/api/ReportRequest`      | Tạo report request mới               |
| PUT    | `/api/ReportRequest/{id}` | Cập nhật report request              |
| DELETE | `/api/ReportRequest/{id}` | Xóa report request                   |

---

## 14. ADMIN CONTROLLERS

### AdminUsersController (`/api/AdminUsers`)

| Method | Endpoint               | Mô tả                       |
| ------ | ---------------------- | --------------------------- |
| GET    | `/api/AdminUsers`      | Lấy danh sách users (Admin) |
| GET    | `/api/AdminUsers/{id}` | Lấy user theo ID (Admin)    |
| POST   | `/api/AdminUsers`      | Tạo user mới (Admin)        |
| PUT    | `/api/AdminUsers/{id}` | Cập nhật user (Admin)       |
| DELETE | `/api/AdminUsers/{id}` | Xóa user (Admin)            |

### AdminDepartmentsController (`/api/AdminDepartments`)

| Method | Endpoint                     | Mô tả                             |
| ------ | ---------------------------- | --------------------------------- |
| GET    | `/api/AdminDepartments`      | Lấy danh sách departments (Admin) |
| GET    | `/api/AdminDepartments/{id}` | Lấy department theo ID (Admin)    |
| POST   | `/api/AdminDepartments`      | Tạo department mới (Admin)        |
| PUT    | `/api/AdminDepartments/{id}` | Cập nhật department (Admin)       |
| DELETE | `/api/AdminDepartments/{id}` | Xóa department (Admin)            |

### AdminDepartmentHeadController (`/api/AdminDepartmentHead`)

| Method | Endpoint                        | Mô tả                                  |
| ------ | ------------------------------- | -------------------------------------- |
| GET    | `/api/AdminDepartmentHead`      | Lấy danh sách department heads (Admin) |
| GET    | `/api/AdminDepartmentHead/{id}` | Lấy department head theo ID (Admin)    |
| POST   | `/api/AdminDepartmentHead`      | Tạo department head mới (Admin)        |
| PUT    | `/api/AdminDepartmentHead/{id}` | Cập nhật department head (Admin)       |
| DELETE | `/api/AdminDepartmentHead/{id}` | Xóa department head (Admin)            |

### AdminNotificationController (`/api/AdminNotification`)

| Method | Endpoint                      | Mô tả                               |
| ------ | ----------------------------- | ----------------------------------- |
| GET    | `/api/AdminNotification`      | Lấy danh sách notifications (Admin) |
| GET    | `/api/AdminNotification/{id}` | Lấy notification theo ID (Admin)    |
| POST   | `/api/AdminNotification`      | Tạo notification mới (Admin)        |
| PUT    | `/api/AdminNotification/{id}` | Cập nhật notification (Admin)       |
| DELETE | `/api/AdminNotification/{id}` | Xóa notification (Admin)            |

### AdminAttachmentController (`/api/AdminAttachment`)

| Method | Endpoint                    | Mô tả                             |
| ------ | --------------------------- | --------------------------------- |
| GET    | `/api/AdminAttachment`      | Lấy danh sách attachments (Admin) |
| GET    | `/api/AdminAttachment/{id}` | Lấy attachment theo ID (Admin)    |
| POST   | `/api/AdminAttachment`      | Tạo attachment mới (Admin)        |
| PUT    | `/api/AdminAttachment/{id}` | Cập nhật attachment (Admin)       |
| DELETE | `/api/AdminAttachment/{id}` | Xóa attachment (Admin)            |

### AdminAttachmentEntityTypeController (`/api/AdminAttachmentEntityType`)

| Method | Endpoint                              | Mô tả                                         |
| ------ | ------------------------------------- | --------------------------------------------- |
| GET    | `/api/AdminAttachmentEntityType`      | Lấy danh sách attachment entity types (Admin) |
| GET    | `/api/AdminAttachmentEntityType/{id}` | Lấy attachment entity type theo ID (Admin)    |
| POST   | `/api/AdminAttachmentEntityType`      | Tạo attachment entity type mới (Admin)        |
| PUT    | `/api/AdminAttachmentEntityType/{id}` | Cập nhật attachment entity type (Admin)       |
| DELETE | `/api/AdminAttachmentEntityType/{id}` | Xóa attachment entity type (Admin)            |

### AdminAuditStatusController (`/api/AdminAuditStatus`)

| Method | Endpoint                     | Mô tả                                |
| ------ | ---------------------------- | ------------------------------------ |
| GET    | `/api/AdminAuditStatus`      | Lấy danh sách audit statuses (Admin) |
| GET    | `/api/AdminAuditStatus/{id}` | Lấy audit status theo ID (Admin)     |
| POST   | `/api/AdminAuditStatus`      | Tạo audit status mới (Admin)         |
| PUT    | `/api/AdminAuditStatus/{id}` | Cập nhật audit status (Admin)        |
| DELETE | `/api/AdminAuditStatus/{id}` | Xóa audit status (Admin)             |

### AdminAuditLogController (`/api/AdminAuditLog`)

| Method | Endpoint                  | Mô tả                            |
| ------ | ------------------------- | -------------------------------- |
| GET    | `/api/AdminAuditLog`      | Lấy danh sách audit logs (Admin) |
| GET    | `/api/AdminAuditLog/{id}` | Lấy audit log theo ID (Admin)    |
| POST   | `/api/AdminAuditLog`      | Tạo audit log mới (Admin)        |
| PUT    | `/api/AdminAuditLog/{id}` | Cập nhật audit log (Admin)       |
| DELETE | `/api/AdminAuditLog/{id}` | Xóa audit log (Admin)            |

### AdminActionStatusController (`/api/AdminActionStatus`)

| Method | Endpoint                      | Mô tả                                 |
| ------ | ----------------------------- | ------------------------------------- |
| GET    | `/api/AdminActionStatus`      | Lấy danh sách action statuses (Admin) |
| GET    | `/api/AdminActionStatus/{id}` | Lấy action status theo ID (Admin)     |
| POST   | `/api/AdminActionStatus`      | Tạo action status mới (Admin)         |
| PUT    | `/api/AdminActionStatus/{id}` | Cập nhật action status (Admin)        |
| DELETE | `/api/AdminActionStatus/{id}` | Xóa action status (Admin)             |

---

## 15. DEPARTMENT HEAD CONTROLLERS

### DepartmentHeadFindingController (`/api/DepartmentHeadFinding`)

| Method | Endpoint                          | Mô tả                                      |
| ------ | --------------------------------- | ------------------------------------------ |
| GET    | `/api/DepartmentHeadFinding`      | Lấy danh sách findings cho Department Head |
| GET    | `/api/DepartmentHeadFinding/{id}` | Lấy finding theo ID cho Department Head    |
| PUT    | `/api/DepartmentHeadFinding/{id}` | Cập nhật finding cho Department Head       |

---

## TỔNG KẾT

- **Tổng số Controllers**: 36 controllers
- **Tổng số API Endpoints**: 207 endpoints
- **Các nhóm chức năng chính**:
  - Authentication & Authorization
  - Audit Management (CRUD + Workflow)
  - Findings Management
  - Actions Management
  - Checklist Management
  - Reports & Export
  - Admin Management
  - Department Head Management

---

**Lưu ý**:

- Tất cả các API đều có base URL: `/api/[ControllerName]`
- Hầu hết các API đều yêu cầu authentication (JWT Token)
- Một số API yêu cầu authorization theo role
- Các API sử dụng soft delete (đánh dấu inactive thay vì xóa thật)
