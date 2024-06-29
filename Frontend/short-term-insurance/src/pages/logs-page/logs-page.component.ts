import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
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


@Component({
  selector: 'app-logs-page',
  standalone: true,
  imports: [MatButtonModule,
    PreventDoubleClick,
    MatPaginatorModule,
    MatIcon,
    MatProgressSpinnerModule,
    MatTableModule],
  templateUrl: './logs-page.component.html',
  styleUrl: './logs-page.component.css'
})
export class LogsPageComponent {
  @ViewChild(MatPaginator) paginator: MatPaginator = new MatPaginator(
    new MatPaginatorIntl(), ChangeDetectorRef.prototype
  );

  dataSource: MatTableDataSource<Logs> = new MatTableDataSource<Logs>();

  displayedColumns: string[] = ['date', 'message'];
  error: boolean = false;
  loading: boolean = true;
  isLastPage: boolean = false;
  page: number = 1;

  constructor(
    private insuranceService: InsuranceService,
    private snackBar: MatSnackBar
  ) { }

  ngOnInit() {
    this.getLogsData();
  }

  getLogsData(nextPage: boolean = false) {
    const endDate = new Date();
    const beginDate = new Date(0);

    this.error = false;
    this.loading = true;
    this.insuranceService.getLogs(beginDate, endDate, this.page)
      .subscribe({
        next: response => {
          this.isLastPage = !response.length;
          if (nextPage && this.isLastPage) {
            this.page--;
            this.snackBar.open('On Last Page.', 'Ok', { "duration": 4000 });
            return;
          }

          this.dataSource = new MatTableDataSource<Logs>(response);
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
