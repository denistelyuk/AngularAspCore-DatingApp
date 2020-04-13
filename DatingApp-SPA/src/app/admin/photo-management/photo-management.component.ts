import { Component, OnInit } from '@angular/core';
import { ModeratePhoto } from 'src/app/_models/moderatePhoto';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  photos: ModeratePhoto[];

  constructor(private adminService: AdminService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getPhotosForModeration();
  }

  getPhotosForModeration() {
    this.adminService.getPhotosForModeration().subscribe((photos: ModeratePhoto[]) => {
      this.photos = photos;
    }, error => {
      this.alertify.error(error);
    });
  }

  approvePhoto(photo: ModeratePhoto) {
    this.adminService.approvePhoto(photo.id).subscribe(() => {
      this.photos = [...this.photos.filter(p => p.id !== photo.id)];
      this.alertify.success('Photo approved');
    }, error => {
      this.alertify.error(error);
    });
  }

  rejectPhoto(photo: ModeratePhoto) {
    this.adminService.rejectPhoto(photo.id).subscribe(() => {
      this.photos = [...this.photos.filter(p => p.id !== photo.id)];
      this.alertify.success('Photo rejected');
    }, error => {
      this.alertify.error(error);
    });
  }

}
