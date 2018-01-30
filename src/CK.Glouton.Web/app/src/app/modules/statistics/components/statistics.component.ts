import { Component, OnInit } from '@angular/core';
import { StatisticsService } from 'app/_services';
import * as palette from './../../../common/js/palette.js';
import { StatisticsData, IChartData } from 'app/common/statistics/models/statistics.model';
import { error } from 'util';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
    selector: 'statistics',
    templateUrl: 'statistics.component.html',
    styleUrls: ['statistics.component.css']
})

export class StatisticsComponent implements OnInit {
    statisticsData: StatisticsData;

    dataChart: IChartData[];

    constructor(private statisticService: StatisticsService, private messageService : MessageService) {
        this.statisticsData = new StatisticsData();
        this.dataChart = new Array<IChartData>();
    }

    constructDataPieChart(data: { [index: string]: number }, legendText = ""): IChartData {
        let keys: string[] = [];
        let value: number[] = [];

        for (var key in data) {
            keys.push(`${key} (${data[key]})`);
            value.push(data[key]);
        }

        let chartData: IChartData = {
            data: {
                labels: keys,
                datasets: [
                    {
                        data: value,
                        backgroundColor: palette.palette('tol-rainbow', keys.length, null, null).map(hex => '#' + hex)
                    }]
            }
        }

        chartData.option = {
            title: {
                display: false,
                text: legendText,
            },
            legend: {
                position: "right",
            }
        }

        return chartData;
    }

    ngOnInit() {
        this.statisticService.getAppNameCount().subscribe(
            d => this.statisticsData.totalAppNameCount = d,
            error => this.messageService.add({
                severity : 'error', summary : 'Error', detail : 'Error while trying to get App Name count' 
            })
        );
        this.statisticService.getAppNames().subscribe(
            d => this.statisticsData.appNames = d,
            error => this.messageService.add({
                severity : 'error', summary : 'Error', detail : 'Error while trying to get App Name' 
            })
        );
        this.statisticService.getTotalExceptionCount().subscribe(
            d => this.statisticsData.totalExceptionCount = d,
            error => this.messageService.add({
                severity : 'error', summary : 'Error', detail : 'Error while trying to get total exception count' 
            })
        );
        this.statisticService.getTotalLogCount().subscribe(
            d => this.statisticsData.totalLogCount = d,
            error => this.messageService.add({
                severity : 'error', summary : 'Error', detail : 'Error while trying to get total log count' 
            })
        );
        this.statisticService.getExceptionCountByAppName().subscribe(d => {
                this.statisticsData.exceptionCountByAppName = d;
                this.dataChart.push(this.constructDataPieChart(this.statisticsData.exceptionCountByAppName, "Exception by AppName"));
            },
            error => this.messageService.add({
                severity : 'error', summary : 'Error', detail : 'Error while trying to get exception count by app name' 
            })
        );
        this.statisticService.getLogCountByAppName().subscribe(d => {
                this.statisticsData.logCountByAppName = d;
                this.dataChart.push(this.constructDataPieChart(this.statisticsData.logCountByAppName, "Log by AppName"));
            },
            error => this.messageService.add({
                severity : 'error', summary : 'Error', detail : 'Error while trying to get log count by app name' 
            })
        );
    }
}