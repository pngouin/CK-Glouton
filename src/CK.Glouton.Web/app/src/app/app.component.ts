import { Component, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app',
  templateUrl: './app.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: [
    '../../node_modules/font-awesome/css/font-awesome.min.css',
    './common/style/theme.scss',
    '../../node_modules/primeng/resources/primeng.min.css',
    './common/style/style.css'
  ]
})
export class AppComponent {
}
