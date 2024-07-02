import { ChangeDetectorRef, Component, ViewChild, ChangeDetectionStrategy, Type } from '@angular/core';
import { MatTableDataSource, MatTableModule } from "@angular/material/table";
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIcon } from '@angular/material/icon';
import { PreventDoubleClick } from '../../directives/prevent-double-click.directive';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Logs } from '../../models/logs.model';
import { MatButtonModule } from '@angular/material/button';
import { InsuranceService } from '../../services/insurance.service';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatFormFieldModule} from '@angular/material/form-field';

import {JsonPipe} from '@angular/common';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import { L } from '@angular/cdk/keycodes';

@Component({
  selector: 'app-logs-page',
  standalone: true,
  imports: [MatButtonModule,
    PreventDoubleClick,
    MatPaginatorModule,
    MatIcon,
    MatProgressSpinnerModule,
    MatTableModule,
    MatFormFieldModule, 
    MatDatepickerModule,
    MatFormFieldModule, 
    MatDatepickerModule, 
    FormsModule, 
    ReactiveFormsModule, 
    JsonPipe],

  templateUrl: './logs-page.component.html',
  styleUrl: './logs-page.component.css',
  providers: [provideNativeDateAdapter()],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsPageComponent {
  @ViewChild(MatPaginator) paginator: MatPaginator = new MatPaginator(
    new MatPaginatorIntl(), ChangeDetectorRef.prototype
  );
  readonly range = new FormGroup({
    start: new FormControl<Date>(new Date()),
    end: new FormControl<Date>(new Date()),
  });

  dataSource: MatTableDataSource<Logs> = new MatTableDataSource<Logs>();

  displayedColumns: string[] = ['date', 'message'];
  error: boolean = false;
  loading: boolean = true;
  isLastPage: boolean = false;
  page: number = 1;
  endDate: Date | null = null;
  beginDate: Date | null = null;

  constructor(
    private insuranceService: InsuranceService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.getLogsData();
  }
   
  
  public filterButtonClicked(){
    this.beginDate = this.range.controls.start.value;
    this.endDate = this.range.controls.end.value;

    this.getLogsData();
   }


  getLogsData(nextPage: boolean = false) {
    if (this.range.controls.start.value){
      this.beginDate = this.range.controls.start.value;
    }
    else {
      this.beginDate = new Date(0);
    }
    if (this.range.controls.end.value){
      this.endDate = this.range.controls.end.value;
    }
    else {
      this.endDate = new Date(0);
    }

    this.error = false;
    this.loading = true;
    this.insuranceService.getLogs(this.beginDate, this.endDate, this.page)
      .subscribe({
        next: response => {
          this.isLastPage = response.page === response.availablePages;
          if (nextPage && this.isLastPage) {
            this.page--;
            this.snackBar.open('On Last Page.', 'Ok', { "duration": 4000 });
            return;
          }

          this.dataSource = new MatTableDataSource<Logs>(response.data);
          this.loading = false;
        },
        error: () => {
          this.error = true;
        },
      })
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
  }

  previousPage() {
    if (this.page === 1) {
      this.snackBar.open('On First Page.', 'Ok', { "duration": 4000 });
      return;
    }
    this.page--;
    this.getLogsData()
  }

  nextPage() {
    this.page++;
    this.getLogsData(true)
  }

}
