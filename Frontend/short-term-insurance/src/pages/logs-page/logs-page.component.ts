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
import {MatInputModule} from '@angular/material/input';


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

  displayedColumns: string[] = ['date', 'message'];
  error: boolean = false;
  loading: boolean = true;
  isLastPage: boolean = false;
  page: number = 1;
  endDate: Date | null = null;
  beginDate: Date | null = null;

  tempDate: Date | null = null;
  tempDateEnd: Date | null = null

  constructor(
    private insuranceService: InsuranceService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit() {
    this.getLogsData();
  }
   
  public buildDate(y: number, m: number , d: number) {
    // Ensure max dates are correct
    if (y < 1 || y > 9999) {
      throw new Error('Year must be between 0001 and 9999');
    }
    if (m < 1 || m > 12) {
      throw new Error('Month must be between 1 and 12');
    }
    if (d < 1 || d > 31) {
      throw new Error('Day must be between 1 and 31');
    }

    const formattedYear = String(y).padStart(4, '0');
    const formattedMonth = String(m).padStart(2, '0');
    const formattedDay = String(d).padStart(2, '0');

    // Create new date string
    const dateString = `${formattedYear}-${formattedMonth}-${formattedDay}T00:00:00.000Z`;

    // Create date object
    return new Date(dateString)
  }


  public filterButtonClicked(){

    console.log('new start date: ' + (this.date.value.startDate));
    console.log('new end date: ' + this.date.value.endDate);

    let year = String(this.date.value.startDate).substring(0, 4);
    let month = String(this.date.value.startDate).substring(5, 7);
    let day = String(this.date.value.startDate).substring(8, 10);

    this.beginDate = this.buildDate(parseInt(year), parseInt(month), parseInt(day));
    console.log('Formatted start date: ' + this.beginDate);
    
    year = String(this.date.value.endDate).substring(0, 4);
    month = String(this.date.value.endDate).substring(5, 7);
    day = String(this.date.value.endDate).substring(8, 10);

    this.endDate = this.buildDate(parseInt(year), parseInt(month), parseInt(day));
    console.log('Formatted end date: ' + this.endDate);

    this.getLogsData();
   }


  getLogsData(nextPage: boolean = false) {
    console.log('getLogsData startDate: ' + this.beginDate + ' ' + this.endDate);
   if (this.date.value.startDate){
      this.beginDate = this.date.value.startDate;
   }
   else {
     this.beginDate = new Date(0);
   }
   if (this.date.value.endDate){
      this.endDate = this.date.value.endDate;
   }
   else {
     this.endDate = new Date(0);
   }
    console.log('getLogsData startDate: ' + this.beginDate + ' ' + this.endDate);

    let strStartDate = new Date(this.beginDate).toISOString().split('T')[0].replace(/-/g, '/');
    console.log('strDate ' + (strStartDate));
    let strEndDate = new Date(this.endDate).toISOString().split('T')[0].replace(/-/g, '/');
    console.log('strDate ' + (strEndDate));

    this.error = false;
    this.loading = true;
    this.insuranceService.getLogs(strStartDate, strEndDate, this.page)
      .subscribe({
        next: response => {
          console.log(response);
          // this.isLastPage = response.page === response.availablePages;
          // if (nextPage && this.isLastPage) {
          //   this.page--;
          //   this.snackBar.open('On Last Page.', 'Ok', { "duration": 4000 });
          //   return;
          // }

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
