import { Component, OnInit } from '@angular/core';
import { StatisticsService } from 'app/_services';
import { StatisticsData } from 'app/common/logs/models';

@Component({
    selector: 'statistics',
    templateUrl: 'statistics.component.html',
    styleUrls: ['statistics.component.css']
})

export class StatisticsComponent implements OnInit {
    statisticsData : StatisticsData;
    constructor(private statisticService: StatisticsService) { 
        this.statisticsData = new StatisticsData();
    }

    constructDataPieChart ( data : { [index: string]: number } ) : any {
        let chartData : any;
        let keys : string[] = [];
        let value : number[] = [];
        
        for (var key in data)
        {
            keys.push(key);
            value.push(data[key]);
        }

        chartData = {
            labels: keys,
            datasets : [
                {
                    data: value,
                }]
        };

        return chartData;
    }

    ngOnInit() {
        this.statisticService.getAppNameCount().subscribe(d => this.statisticsData.totalAppNameCount = d);
        this.statisticService.getAppNames().subscribe(d => this.statisticsData.appNames = d);
        this.statisticService.getTotalExceptionCount().subscribe(d => this.statisticsData.totalExceptionCount = d);
        this.statisticService.getTotalLogCount().subscribe(d => this.statisticsData.totalLogCount = d);
        this.statisticService.getExceptionCountByAppName().subscribe(d => this.statisticsData.exceptionCountByAppName = d);
        this.statisticService.getLogCountByAppName().subscribe(d => this.statisticsData.logCountByAppName = d);
     }
}