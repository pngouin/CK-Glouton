import { Component, OnInit } from '@angular/core';
import { StatisticsService } from 'app/_services';
import * as palette from './../../../common/js/palette.js';
import { StatisticsData, IChartData } from 'app/common/statistics/models/statistics.model';

@Component({
    selector: 'statistics',
    templateUrl: 'statistics.component.html',
    styleUrls: ['statistics.component.css']
})

export class StatisticsComponent implements OnInit {
    statisticsData: StatisticsData;

    dataChart: IChartData[];

    constructor(private statisticService: StatisticsService) {
        this.statisticsData = new StatisticsData();
        this.dataChart = new Array<IChartData>();
    }

    constructDataPieChart(data: { [index: string]: number }, legendText = ""): IChartData {
        let keys: string[] = [];
        let value: number[] = [];

        for (var key in data) {
            keys.push(key);
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
                display: true,
                text: legendText
            }
        }

        return chartData;
    }

    ngOnInit() {
        this.statisticService.getAppNameCount().subscribe(d => this.statisticsData.totalAppNameCount = d);
        this.statisticService.getAppNames().subscribe(d => this.statisticsData.appNames = d);
        this.statisticService.getTotalExceptionCount().subscribe(d => this.statisticsData.totalExceptionCount = d);
        this.statisticService.getTotalLogCount().subscribe(d => this.statisticsData.totalLogCount = d);
        this.statisticService.getExceptionCountByAppName().subscribe(d => {
            this.statisticsData.exceptionCountByAppName = d;
            this.dataChart.push(this.constructDataPieChart(this.statisticsData.exceptionCountByAppName, "Exception by AppName"));
        });
        this.statisticService.getLogCountByAppName().subscribe(d => {
            this.statisticsData.logCountByAppName = d;
            this.dataChart.push(this.constructDataPieChart(this.statisticsData.logCountByAppName, "Log by AppName"));
        });
    }
}