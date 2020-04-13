import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

constructor(private http: HttpClient) { }

  getUsersWithRoles(){
    return this.http.get(this.baseUrl + 'admin/usersWithRoles');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'admin/editRoles/' + user.username, roles);
  }

  getAvailableRoles() {
    return  [
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'},
      {name: 'VIP', value: 'VIP'},
    ];
  }

  rejectPhoto(photoId: number) {
    return this.http.post(this.baseUrl + 'admin/photo/' + photoId + '/reject', {});
  }

  approvePhoto(photoId: number) {
    return this.http.post(this.baseUrl + 'admin/photo/' + photoId + '/approve', {});
  }

  getPhotosForModeration() {
    return this.http.get(this.baseUrl + 'admin/photosForModeration');
  }
  
}
