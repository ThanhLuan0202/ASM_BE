# DANH SÁCH API - AUDIT MANAGEMENT SYSTEM FOR AVIATION ACADEMY

## TỔNG HỢP TẤT CẢ API ENDPOINTS (207 endpoints)

### 1. AUTHENTICATION & AUTHORIZATION

POST /api/Auth/login
POST /api/Auth/register
POST /api/Auth/bulk-register
POST /api/Auth/reset-password

### 2. AUDIT MANAGEMENT

GET /api/Audits
GET /api/Audits/{id}
POST /api/Audits
PUT /api/Audits/{id}
DELETE /api/Audits/{id}
POST /api/Audits/{id}/submit-to-lead-auditor
POST /api/Audits/{id}/approve-forward-director
POST /api/Audits/{id}/declined-plan-content
POST /api/Audits/{id}/approve-plan
POST /api/Audits/{id}/reject-plan-content
GET /api/Audits/Summary/{auditId}
POST /api/Audits/Submit/{auditId}
PUT /api/Audits/archive/{auditId}
PUT /api/Audits/{id}/complete-update
GET /api/Audits/{auditId}/chart/line
GET /api/Audits/{auditId}/chart/pie
GET /api/Audits/{auditId}/chart/bar
GET /api/Audits/ExportPdf/{auditId}
GET /api/Audits/ExportExcel/{auditId}
GET /api/Audits/by-period
POST /api/Audits/validate-department
GET /api/Audits/period-status
GET /api/AuditPlan
GET /api/AuditPlan/{auditId}
PUT /api/AuditPlan/{auditId}
GET /api/AuditPlanAssignment
GET /api/AuditPlanAssignment/{id}
POST /api/AuditPlanAssignment
PUT /api/AuditPlanAssignment/{id}
DELETE /api/AuditPlanAssignment/{id}
GET /api/AuditPlanAssignment/by-period
POST /api/AuditPlanAssignment/validate

### 3. AUDIT ASSIGNMENT

GET /api/AuditAssignment
GET /api/AuditAssignment/{assignmentId}
GET /api/AuditAssignment/audit/{auditId}
GET /api/AuditAssignment/auditor/{auditorId}
GET /api/AuditAssignment/department/{deptId}
GET /api/AuditAssignment/my-assignments
POST /api/AuditAssignment
PUT /api/AuditAssignment/{assignmentId}
DELETE /api/AuditAssignment/{assignmentId}
GET /api/AuditAssignment/auditors-with-schedule

### 4. AUDIT TEAM

GET /api/AuditTeam
GET /api/AuditTeam/{id}
POST /api/AuditTeam
PUT /api/AuditTeam/{id}
DELETE /api/AuditTeam/{id}
GET /api/AuditTeam/check-lead-auditor/{auditId}

### 5. AUDIT SCOPE & SCHEDULE

GET /api/AuditScopeDepartment
GET /api/AuditScopeDepartment/{id}
POST /api/AuditScopeDepartment
PUT /api/AuditScopeDepartment/{id}
DELETE /api/AuditScopeDepartment/{id}
GET /api/AuditSchedule
GET /api/AuditSchedule/{id}
POST /api/AuditSchedule
PUT /api/AuditSchedule/{id}
DELETE /api/AuditSchedule/{id}

### 6. FINDINGS

GET /api/Findings
GET /api/Findings/{id}
POST /api/Findings
PUT /api/Findings/{id}
DELETE /api/Findings/{id}
GET /api/Findings/by-department/{deptId}
GET /api/Findings/by-audit-item/{auditItemId}
PUT /api/Findings/{findingId}/received
GET /api/Findings/by-audit/{auditId}
GET /api/Findings/by-created-by/{createdBy}
GET /api/Findings/my-findings
GET /api/FindingStatus
GET /api/FindingStatus/{status}
POST /api/FindingStatus
PUT /api/FindingStatus/{status}
DELETE /api/FindingStatus/{status}
GET /api/FindingSeverity
GET /api/FindingSeverity/{severity}
POST /api/FindingSeverity
PUT /api/FindingSeverity/{severity}
DELETE /api/FindingSeverity/{severity}

### 7. ACTIONS

GET /api/Action
GET /api/Action/{id}
GET /api/Action/my-assigned
POST /api/Action
PUT /api/Action/{id}
DELETE /api/Action/{id}
POST /api/Action/{id}/status/in-progress
POST /api/Action/{id}/status/reviewed
POST /api/Action/{id}/status/approved
POST /api/Action/{id}/status/rejected
PUT /api/Action/{id}/progress-percent
POST /api/Action/{id}/status/closed
PUT /api/Action/{id}/approve-by-auditor
GET /api/Action/by-finding/{findingId}
GET /api/Action/by-assigned-dept/{assignedDeptId}
PUT /api/Action/{id}/reject
POST /api/ActionReview/{actionId}/verified
POST /api/ActionReview/{actionId}/declined

### 8. CHECKLIST MANAGEMENT

GET /api/ChecklistTemplates
GET /api/ChecklistTemplates/{id}
POST /api/ChecklistTemplates
PUT /api/ChecklistTemplates/{id}
DELETE /api/ChecklistTemplates/{id}
GET /api/ChecklistItems
GET /api/ChecklistItems/{id}
POST /api/ChecklistItems
PUT /api/ChecklistItems/{id}
DELETE /api/ChecklistItems/{id}
GET /api/ChecklistItemNoFinding
GET /api/ChecklistItemNoFinding/{id}
POST /api/ChecklistItemNoFinding
PUT /api/ChecklistItemNoFinding/{id}
DELETE /api/ChecklistItemNoFinding/{id}
GET /api/AuditChecklistItems
GET /api/AuditChecklistItems/{id}
POST /api/AuditChecklistItems
PUT /api/AuditChecklistItems/{id}
DELETE /api/AuditChecklistItems/{id}
GET /api/AuditChecklistTemplateMaps
GET /api/AuditChecklistTemplateMaps/{id}
POST /api/AuditChecklistTemplateMaps
PUT /api/AuditChecklistTemplateMaps/{id}
DELETE /api/AuditChecklistTemplateMaps/{id}

### 9. AUDIT CRITERIA

GET /api/AuditCriterion
GET /api/AuditCriterion/{id}
POST /api/AuditCriterion
PUT /api/AuditCriterion/{id}
DELETE /api/AuditCriterion/{id}
GET /api/AuditCriteriaMap
GET /api/AuditCriteriaMap/{id}
POST /api/AuditCriteriaMap
PUT /api/AuditCriteriaMap/{id}
DELETE /api/AuditCriteriaMap/{id}

### 10. AUDIT APPROVAL & DOCUMENTS

GET /api/AuditApproval
GET /api/AuditApproval/{id}
POST /api/AuditApproval
PUT /api/AuditApproval/{id}
DELETE /api/AuditApproval/{id}
GET /api/AuditDocuments
GET /api/AuditDocuments/{id}

### 11. ROOT CAUSES

GET /api/RootCauses
GET /api/RootCauses/{id}
POST /api/RootCauses
PUT /api/RootCauses/{id}
DELETE /api/RootCauses/{id}

### 12. ROLES

GET /api/Role
GET /api/Role/{roleName}
POST /api/Role
PUT /api/Role/{roleName}
DELETE /api/Role/{roleName}

### 13. REPORTS

GET /api/AuditReports
GET /api/ReportRequest
GET /api/ReportRequest/{id}
POST /api/ReportRequest
PUT /api/ReportRequest/{id}
DELETE /api/ReportRequest/{id}

### 14. ADMIN CONTROLLERS

GET /api/AdminUsers
GET /api/AdminUsers/{id}
POST /api/AdminUsers
PUT /api/AdminUsers/{id}
DELETE /api/AdminUsers/{id}
GET /api/AdminDepartments
GET /api/AdminDepartments/{id}
POST /api/AdminDepartments
PUT /api/AdminDepartments/{id}
DELETE /api/AdminDepartments/{id}
GET /api/AdminDepartmentHead
GET /api/AdminDepartmentHead/{id}
POST /api/AdminDepartmentHead
PUT /api/AdminDepartmentHead/{id}
DELETE /api/AdminDepartmentHead/{id}
GET /api/AdminNotification
GET /api/AdminNotification/{id}
POST /api/AdminNotification
PUT /api/AdminNotification/{id}
DELETE /api/AdminNotification/{id}
GET /api/AdminAttachment
GET /api/AdminAttachment/{id}
POST /api/AdminAttachment
PUT /api/AdminAttachment/{id}
DELETE /api/AdminAttachment/{id}
GET /api/AdminAttachmentEntityType
GET /api/AdminAttachmentEntityType/{id}
POST /api/AdminAttachmentEntityType
PUT /api/AdminAttachmentEntityType/{id}
DELETE /api/AdminAttachmentEntityType/{id}
GET /api/AdminAuditStatus
GET /api/AdminAuditStatus/{id}
POST /api/AdminAuditStatus
PUT /api/AdminAuditStatus/{id}
DELETE /api/AdminAuditStatus/{id}
GET /api/AdminAuditLog
GET /api/AdminAuditLog/{id}
POST /api/AdminAuditLog
PUT /api/AdminAuditLog/{id}
DELETE /api/AdminAuditLog/{id}
GET /api/AdminActionStatus
GET /api/AdminActionStatus/{id}
POST /api/AdminActionStatus
PUT /api/AdminActionStatus/{id}
DELETE /api/AdminActionStatus/{id}

### 15. DEPARTMENT HEAD CONTROLLERS

GET /api/DepartmentHeadFinding
GET /api/DepartmentHeadFinding/{id}
PUT /api/DepartmentHeadFinding/{id}

---

**TỔNG KẾT: 207 API Endpoints**
