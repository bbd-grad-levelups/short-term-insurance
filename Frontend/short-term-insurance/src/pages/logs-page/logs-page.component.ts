import { ChangeDetectorRef, Component, ViewChild } from '@angular/core';
import {MatTableDataSource, MatTableModule} from "@angular/material/table";
import { MatPaginator, MatPaginatorIntl, MatPaginatorModule } from '@angular/material/paginator';
import { InsuranceService } from '../../services/insurance.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIcon } from '@angular/material/icon';
import { PreventDoubleClick } from '../../directives/prevent-double-click.directive';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Persona } from '../../models/persona.model';
import { Logs } from '../../models/logs.model';
import { LogsService } from '../../services/logs.service';


@Component({
  selector: 'app-logs-page',
  standalone: true,
  imports: [    PreventDoubleClick,
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
    private logsService: LogsService,
    private snackBar: MatSnackBar 
  ) {}

  ngOnInit() {
    this.getLogsData();
  }

  getLogsData(nextPage: boolean = false) {
    this.error = false;
    this.loading = true;
    this.logsService.getLogs(this.page)
      .subscribe({
        next: response => {
          if (response.success) {
            this.isLastPage = !response.data.length;
            if (nextPage && this.isLastPage) {
              this.page--;
              this.snackBar.open('On Last Page.', 'Ok', {"duration": 4000});
              return;
            }

            this.dataSource = new MatTableDataSource<Logs>(response.data);
            this.loading = false;

          } else {
            this.error = true;
          }
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
      this.snackBar.open('On First Page.', 'Ok', {"duration": 4000});
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
