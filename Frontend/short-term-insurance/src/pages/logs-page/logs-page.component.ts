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
import { provideNativeDateAdapter } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';

import { JsonPipe } from '@angular/common';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { L } from '@angular/cdk/keycodes';
import { MatInputModule } from '@angular/material/input';

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
    JsonPipe,
    MatInputModule],

  templateUrl: './logs-page.component.html',
  styleUrl: './logs-page.component.css',
  providers: [provideNativeDateAdapter()],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LogsPageComponent {
  @ViewChild(MatPaginator) paginator: MatPaginator = new MatPaginator(
    new MatPaginatorIntl(), ChangeDetectorRef.prototype
  );
  today = new Date();
  aWeekAgo = this.today.getDate() - 7;
  tempWeek = new Date();
  initLastWeek = new Date(this.tempWeek.setDate(this.aWeekAgo));
  // readonly range = new FormGroup({
  //   start: new FormControl<Date>(this.initLastWeek),
  //   end: new FormControl<Date>(new Date()),
  // });

  readonly date = new FormGroup({
    startDate: new FormControl<Date>(this.initLastWeek),
    endDate: new FormControl<Date>(new Date())
    });

  dataSource: MatTableDataSource<Logs> = new MatTableDataSource<Logs>();

  displayedColumns: string[] = ['timeStamp', 'message'];
  error: boolean = false;
  loading: boolean = true;
  isLastPage: boolean = false;
  isFirstPage: boolean = true;
  page: number = 1;
  availablePages: number = 1;
  endDate: Date | null = null;
  beginDate: Date | null = null;

  startDateFormControl = new FormControl('0001/01/01', [Validators.required]);
  endDateFormControl = new FormControl('0001/01/05', [Validators.required]);

  constructor(
    private cdr: ChangeDetectorRef,
    private insuranceService: InsuranceService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    this.getLogsData();
  }


  checkDate(start: any, end:any){
    const startDate = new Date(start);
    const endDate = new Date(end);
    if (startDate >= endDate) {
      return false;
    }
    return true;
  }

  submitFilter() {
    if (this.startDateFormControl.invalid || this.endDateFormControl.invalid) return;
    if (this.checkDate(this.startDateFormControl.value, this.endDateFormControl.value) == false) return;
    this.getLogsData()
  }

  getLogsData() {
    this.error = false;
    this.loading = true;
    this.insuranceService.getLogs(this.startDateFormControl.value!, this.endDateFormControl.value!, this.page)
      .subscribe({
        next: response => {
          this.isFirstPage = response.page === 1;
          this.isLastPage = response.page === response.availablePages;
          this.availablePages = response.availablePages;
          this.dataSource = new MatTableDataSource<Logs>(response.data);
          this.loading = false;
          this.cdr.detectChanges();
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
    this.getLogsData()
  }

}
