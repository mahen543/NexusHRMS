export interface Employee {
  id: number;
  employeeGuid: string;
  firstName: string;
  lastName: string;
  email: string;
  designationId: number;
  hierarchyLevel: number;
}

export interface CreateEmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
  designationId: number;
  hierarchyLevel: number;
}

export interface UpdateEmployeeRequest {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  designationId: number;
  hierarchyLevel: number;
}
