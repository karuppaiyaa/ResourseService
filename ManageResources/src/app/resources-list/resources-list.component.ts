import { HttpErrorResponse } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ResourceService } from '../services/ResourceService';
import { AlertService } from '../services/alert.service';

@Component({
  selector: 'app-resources-list',
  templateUrl: './resources-list.component.html',
  styleUrls: ['./resources-list.component.css']
})
export class ResourcesListComponent {

  gridView: { data: any[], total: number } = { data: [], total: 0 };
  searchResourceName = ""
  resourcePath = ""
  resources: any;
  selectedResources: any;
  selectedResourceIds: Set<number> = new Set<number>();
  selectedStatus: Map<number, boolean> = new Map<number, boolean>();
  selectAll: boolean = false;

  constructor(private service: ResourceService, private alertService: AlertService) { }

  ngOnInit(): void {
    this.getResources()
  }

  toggleSelectAll() {
    for (let resource of this.gridView.data) {
      this.selectedStatus.set(resource.id, this.selectAll);
      if (this.selectAll) {
        this.selectedResourceIds.add(resource.id);
      } else {
        this.selectedResourceIds.delete(resource.id);
      }
    }
  }

  toggleResourceSelection(resourceId: number) {
    this.selectedStatus.set(resourceId, !this.selectedStatus.get(resourceId));
    if (this.selectedStatus.get(resourceId)) {
      this.selectedResourceIds.add(resourceId);
    } else {
      this.selectedResourceIds.delete(resourceId);
    }
    this.updateSelectAllStatus();
  }

  updateSelectAllStatus() {
    this.selectAll = this.gridView.data.length > 0 && this.gridView.data.every((resource: any) => this.selectedStatus.get(resource.id));
  }

  public getResources(): void {
    let data = {
      "searchResourceName": this.searchResourceName
    }
    this.gridView.data = [];
    this.gridView.total = 0
    this.service.getAllResources(data).subscribe((res: any) => {
      this.gridView.data = res.resources
      this.gridView.total = res.totalCount
    }, (err: HttpErrorResponse) => {
      this.alertService.error('Unable to retrieve resources', { keepAfterRouteChange: false, autoClose: true });
      this.gridView.data = [];
      this.gridView.total = 0
    });
  }

  onResourceSelected(event: any) {
    this.selectedResources = event.target.files;
  }

  uploadResources() {
    if (this.selectedResources != undefined && this.selectedResources != null && this.selectedResources?.length > 0) {
      const formData = new FormData();
      if (this.selectedResources.length == 1) {
        formData.append('resourcePath', this.resourcePath);
        formData.append('resource', this.selectedResources[0] as File);
        this.service.createResource(formData).subscribe(res => {
          if (res) {
            this.alertService.success('Resource uploaded successfully', { keepAfterRouteChange: false, autoClose: true });
            this.searchResourceName = ''
            this.getResources()
          }
        }, (err: HttpErrorResponse) => {
          this.alertService.error('Unable to upload resource', { keepAfterRouteChange: false, autoClose: true });
        });
      }
      else {
        formData.append('resourcePath', this.resourcePath);
        for (let i = 0; i < this.selectedResources.length; i++) {
          formData.append('resources', this.selectedResources[i] as File);
        }
        this.service.createResources(formData).subscribe(res => {
          if (res) {
            this.alertService.success('Resources uploaded successfully', { keepAfterRouteChange: false, autoClose: true });
            this.searchResourceName = ''
            this.getResources()
          }
        }, (err: HttpErrorResponse) => {
          this.alertService.error('Unable to upload resources', { keepAfterRouteChange: false, autoClose: true });
        });
      }
    } else {
      this.alertService.error('No resources selected', { keepAfterRouteChange: false, autoClose: true });
    }
  }

  getResourceSize(id: any) {
    this.service.getResourceById(id).subscribe(res => {
      this.alertService.success(`Requested resource - ${res.name} size is ${(res.sizeInBytes / (1024 * 1024)).toFixed(3)} MB`, { keepAfterRouteChange: false, autoClose: true });
    }, (err: HttpErrorResponse) => {
      this.alertService.error('Unable to retrieve resource size', { keepAfterRouteChange: false, autoClose: true });
    });
  }

  deleteResource(id: any) {
    this.service.deleteResource(id).subscribe(res => {
      //this.gridView.data = this.gridView.data.filter((item: any) => item.id !== id);
      this.alertService.success('Resource deleted successfully', { keepAfterRouteChange: false, autoClose: true });
      this.selectedResourceIds.clear()
      this.searchResourceName = ''
      this.getResources()
    }, (err: HttpErrorResponse) => {
      this.alertService.error('Unable to delete resource', { keepAfterRouteChange: false, autoClose: true });
    });
  }

  bulkDeleteResources() {
    if (this.selectedResourceIds.size === 0) {
      this.alertService.error('No resources selected', { keepAfterRouteChange: false, autoClose: true });
      return;
    }
    var resources: any[] = [];
    this.selectedResourceIds.forEach((id: any) => {
      resources.push(id as string)
    });
    if (resources.length == 1) {
      this.deleteResource(resources[0])
      return
    }
    this.service.deleteResources({
      "resourceIds": resources,
    }).subscribe(res => {
      this.alertService.success('Resources deleted successfully', { keepAfterRouteChange: false, autoClose: true });
      this.selectedResourceIds.clear()
      this.searchResourceName = ''
      this.getResources()
    }, (err: HttpErrorResponse) => {
      this.alertService.error('Unable to delete resources', { keepAfterRouteChange: false, autoClose: true });
    });
  }

  viewResource(id: any) {
    this.service.getFullPaths({
      "resourceIds": [
        id
      ],
    }).subscribe(res => {
      if (res && res.length > 0) {
        window.open(res[0].fullResourcePath, '_blank');
        this.alertService.success('Resource retrieved successfully', { keepAfterRouteChange: false, autoClose: true });
      }
    }, (err: HttpErrorResponse) => {
      this.alertService.error('Unable to view resource', { keepAfterRouteChange: false, autoClose: true });
    });
  }

  downloadResource(id: any, name = "") {
    this.service.downloadResource(id).subscribe((res: any) => {
      const a = document.createElement('a');
      const objectURL = URL.createObjectURL(res);
      a.href = objectURL;
      a.download = name;
      a.click();
      URL.revokeObjectURL(objectURL);
      this.alertService.success('Resource downloaded successfully', { keepAfterRouteChange: false, autoClose: true });

    }, (err: HttpErrorResponse) => {
      this.alertService.error('Unable to download resource', { keepAfterRouteChange: false, autoClose: true });
    });
  }

  bulkDownloadResources() {
    if (this.selectedResourceIds.size === 0) {
      this.alertService.error('No resources selected', { keepAfterRouteChange: false, autoClose: true });
      return;
    }
    var resources: any[] = [];
    this.selectedResourceIds.forEach((id: any) => {
      resources.push(id)
    });
    if (resources.length == 1) {
      var selectedResource = this.gridView.data.find(x => x.id == resources[0])
      if (selectedResource != null)
        this.downloadResource(resources[0], selectedResource.name)
      return
    }
    this.service.downloadResources({
      "resourceIds": resources,
    }).subscribe((res: any) => {
      const a = document.createElement('a');
      const objectURL = URL.createObjectURL(res);
      a.href = objectURL;
      a.download = "DownloadedResources.zip";
      a.click();
      URL.revokeObjectURL(objectURL);
      this.alertService.success('Resources downloaded successfully', { keepAfterRouteChange: false, autoClose: true });

    }, (err: HttpErrorResponse) => {
      this.alertService.error('Unable to download resources', { keepAfterRouteChange: false, autoClose: true });
    });
  }
}
