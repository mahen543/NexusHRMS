import { CanActivateFn } from '@angular/router';

export const hierarchyGuard: CanActivateFn = (route, state) => {
  return true;
};
