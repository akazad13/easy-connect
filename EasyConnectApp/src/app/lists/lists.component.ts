import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from '../models/pagination';
import { User } from '../models/user';
import { UserService } from '../services/user.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';
import { FormsModule } from '@angular/forms';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { MemberCardComponent } from '../members/member-list/member-card/member-card.component';
import { NgClass, NgFor } from '@angular/common';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
  imports: [FormsModule, PaginationModule, MemberCardComponent, NgFor, NgClass],
  standalone: true,
})
export class ListsComponent implements OnInit {
  users: User[] | null = null;
  pagination: Pagination = {
    currentPage: 1,
    itemsPerPage: 10,
    totalItems: 0,
    totalPages: 0,
  };
  bookmarkParam: string = 'bookmarkeds';
  constructor(
    private readonly userService: UserService,
    private readonly route: ActivatedRoute,
    private readonly alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe((data) => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
    });
    this.bookmarkParam = 'bookmarkeds';
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers(null);
  }

  loadUsers(bookmarkOption: string | null) {
    if (bookmarkOption != null) {
      this.bookmarkParam = bookmarkOption;
    }
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.bookmarkParam).subscribe(
      (res: PaginatedResult<User[]>) => {
        if (res.result != null) {
          this.users = res.result;
        }
        this.pagination = res.pagination;
      },
      (error) => {
        this.alertify.error(error);
      }
    );
  }
}
