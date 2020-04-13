import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {

  users: User[];
  bsModalRef: BsModalRef;

  constructor(private adminServices: AdminService,
              private alertify: AlertifyService,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminServices.getUsersWithRoles().subscribe((users: User[]) => {
      this.users = users;
    }, error => {
      this.alertify.error(error);
    });
  }

  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getUserRoles(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    this.bsModalRef.content.updateRolesEvent.subscribe((roles) => {

      const rolesToUpdate = {
        roleNames: [...roles.filter(r => r.isChecked === true).map(role => role.name)]
      };

      if(rolesToUpdate) {
        this.adminServices.updateUserRoles(user, rolesToUpdate).subscribe(() => {
          user.roles = [...rolesToUpdate.roleNames];
          this.alertify.success(user.username + '\'s roles successfully updated!');
        }, error => {
          this.alertify.error(error);
        });
      }
    });
  }

  getUserRoles(user: User): any[] {
    const roles: any[] = [];
    const availableRoles: any[] = this.adminServices.getAvailableRoles();

    for (const availableRole of availableRoles) {
      const role = availableRole;
      role.isChecked = user.roles.includes(availableRole.name);
      roles.push(role);
    }
    return roles;
  }
}
