import { Component, TemplateRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import {
  TodoListsClient, TodoItemsClient,
  TodoListDto, TodoItemDto, PriorityLevelDto,
  CreateTodoListCommand, UpdateTodoListCommand,
  CreateTodoItemCommand, UpdateTodoItemDetailCommand, UpdateBackgroundTodoItemCommand, TagClient, CreateTagCommand, TagDto, TagBriefDto
} from '../web-api-client';
import { AuthorizeService } from '../../api-authorization/authorize.service';
import { debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { Observable, Subject } from 'rxjs';
import { Debounce } from 'lodash-decorators';

@Component({
  selector: 'app-todo-component',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class TodoComponent implements OnInit {
  //View Child
  @ViewChild('mostUsedTagsModalTemplate') mostUsedTagsModalTemplate: TemplateRef<any>;

  public isAuthenticated?: Observable<boolean>;
  debug = false;
  deleting = false;
  deleteCountDown = 0;
  deleteCountDownInterval: any;
  lists: TodoListDto[];
  priorityLevels: PriorityLevelDto[];
  selectedList: TodoListDto;
  selectedItem: TodoItemDto;
  uniqueListTags: TagBriefDto[];
  newListEditor: any = {};
  listOptionsEditor: any = {};
  newListModalRef: BsModalRef;
  listOptionsModalRef: BsModalRef;
  deleteListModalRef: BsModalRef;
  itemDetailsModalRef: BsModalRef;
  addTagModalRef: BsModalRef;
  mostUsedTagsModalRef: BsModalRef;
  itemDetailsFormGroup = this.fb.group({
    id: [null],
    listId: [null],
    priority: [''],
    note: [''],
  });
  //My  object defined properties
  mostUsedTags: TagDto[] = [];
  newTagEditor: any = {};
  tagFilterFormGroup = this.tagFilterFB.group({
    id: [null],
  });
  changeBackgroundFormGroup = this.changeBGFB.group({
    id: [null],
    colour: [''],
  });
  selectedTag = "all";
  searchedTitle = '';
  colours = ['#FFFFFF', '#FF5733', '#FFC300', '#FFFF66', '#CCFF99', '#6666FF', '#9966CC', '#999999'];


  constructor(
    private listsClient: TodoListsClient,
    private itemsClient: TodoItemsClient,
    private tagsClient: TagClient,
    private modalService: BsModalService,
    private fb: FormBuilder,
    private changeBGFB: FormBuilder,
    private tagFilterFB: FormBuilder,
    private authorizeService: AuthorizeService
  ) { }
  ngOnInit(): void {
    //MY ADDED CODE
    let isUserAuthenticated = false;
    this.isAuthenticated = this.authorizeService.isAuthenticated();
    this.isAuthenticated.subscribe(s => {
      isUserAuthenticated = s;
    });
    this.listsClient.get().subscribe(
      result => {
        if (isUserAuthenticated) {
          document.addEventListener('keydown', (ev) => {
            if (ev.ctrlKey && ev.key == 'k') {
              ev.preventDefault();
              this.showMostUsedTagsModal();
            }
          });
        }
        this.lists = result.lists;
        this.priorityLevels = result.priorityLevels;
        if (this.lists.length) {
          this.selectedList = this.lists[0];
          if (isUserAuthenticated)
            this.getUniqeTags(this.selectedList);
        }
      },
      error => console.error(error)
    );
  }

  // Lists
  remainingItems(list: TodoListDto): number {
    return list.items.filter(t => !t.done).length;
  }

  showNewListModal(template: TemplateRef<any>): void {
    this.newListModalRef = this.modalService.show(template);
    setTimeout(() => document.getElementById('title').focus(), 250);
  }

  newListCancelled(): void {
    this.newListModalRef.hide();
    this.newListEditor = {};
  }

  addList(): void {
    const list = {
      id: 0,
      title: this.newListEditor.title,
      items: []
    } as TodoListDto;

    this.listsClient.create(list as CreateTodoListCommand).subscribe(
      result => {
        list.id = result;
        this.lists.push(list);
        this.selectedList = list;
        this.newListModalRef.hide();
        this.newListEditor = {};
      },
      error => {
        const errors = JSON.parse(error.response);

        if (errors && errors.Title) {
          this.newListEditor.error = errors.Title[0];
        }

        setTimeout(() => document.getElementById('title').focus(), 250);
      }
    );
  }

  showListOptionsModal(template: TemplateRef<any>) {
    this.listOptionsEditor = {
      id: this.selectedList.id,
      title: this.selectedList.title
    };

    this.listOptionsModalRef = this.modalService.show(template);
  }

  updateListOptions() {
    const list = this.listOptionsEditor as UpdateTodoListCommand;
    this.listsClient.update(this.selectedList.id, list).subscribe(
      () => {
        (this.selectedList.title = this.listOptionsEditor.title),
          this.listOptionsModalRef.hide();
        this.listOptionsEditor = {};
      },
      error => console.error(error)
    );
  }

  confirmDeleteList(template: TemplateRef<any>) {
    this.listOptionsModalRef.hide();
    this.deleteListModalRef = this.modalService.show(template);
  }

  deleteListConfirmed(): void {
    this.listsClient.delete(this.selectedList.id).subscribe(
      () => {
        this.deleteListModalRef.hide();
        this.lists = this.lists.filter(t => t.id !== this.selectedList.id);
        this.selectedList = this.lists.length ? this.lists[0] : null;
      },
      error => console.error(error)
    );
  }

  // Items
  showItemDetailsModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.selectedItem = item;
    this.itemDetailsFormGroup.patchValue(this.selectedItem);

    this.itemDetailsModalRef = this.modalService.show(template);
    this.itemDetailsModalRef.onHidden.subscribe(() => {
      this.stopDeleteCountDown();
    });
  }
  //MY METHODS
  //Search for todo item
  searchTodoItem() {
    if (this.isAuthenticated) {
      this.itemsClient.search(this.searchedTitle, this.selectedList.id).subscribe(success => {
      this.selectedList.items = success;
    }, errorResponse => alert(errorResponse) );
    }
  }
  //Select current list
  selectList(list: TodoListDto) {
    this.selectedList = list;
    this.getUniqeTags(list);

  }
  //Get Unique Tags
  getUniqeTags(list: TodoListDto) {
    this.tagsClient.getUniqueTags(list.id).subscribe(
      result => {
        this.uniqueListTags = result;
      },
      error => console.error(error)
    );
  }
  updateItemBackground(itemDTO: TodoItemDto): void {
    this.changeBackgroundFormGroup.get('id').setValue(itemDTO.id);
    const item = new UpdateBackgroundTodoItemCommand(this.changeBackgroundFormGroup.value);
    this.itemsClient.updateItemBackground(itemDTO.id, item).subscribe(() => {
      itemDTO.colour = item.colour;
    }, error => alert(error));
  }
  //Show the add tag modal
  showMostUsedTagsModal(): void {
    this.tagsClient.getMostUsedTags().subscribe(success => {
      this.mostUsedTags = success;
    }, errorResponse => {

    });
    this.mostUsedTagsModalRef = this.modalService.show(this.mostUsedTagsModalTemplate);
  }
  closeMostUsedTagsModal(): void {
    this.mostUsedTagsModalRef.hide();
  }
  showAddTagModal(template: TemplateRef<any>, item: TodoItemDto): void {
    this.selectedItem = item;
    this.addTagModalRef = this.modalService.show(template);
    this.addTagModalRef.onHidden.subscribe(() => {
      this.newTagEditor.name = "";
    });
  }
  //Add the Tag
  addTag() {
    const tag = {
      id: 0,
      todoItemId: this.selectedItem.id,
      name: this.newTagEditor.name,
    } as TagDto;

    this.tagsClient.create(tag as CreateTagCommand).subscribe(
      result => {
        tag.id = result;
        const uniqueListToAdd = new TagBriefDto({ id: tag.id, name: tag.name })
        { this.uniqueListTags.filter(t => t.name == tag.name).length == 0 && this.uniqueListTags.push(tag); }
        this.selectedItem.tags.push(tag);
        this.addTagModalRef.hide();
        this.newTagEditor = {};
      },
      response => {
        const responseStr = JSON.stringify(response);
        const parseResponse = JSON.parse(responseStr);
        const actualResponse = JSON.stringify(parseResponse.response);
        const secondParse = JSON.parse(actualResponse);
        const secondStringify = JSON.stringify(secondParse);
        const thirdParse = JSON.parse(secondStringify);
        const finalParse = JSON.parse(thirdParse);
        const errors = finalParse.errors;

        if (errors) {
          this.newTagEditor.error = errors.Name[0];
        }
        setTimeout(() => document.getElementById('tagName').focus(), 250);
      }
    );
  }

  //Remove the tag
  deleteTag(tagId: number, item: TodoItemDto): void {
    this.tagsClient.delete(tagId).subscribe(() => {
      item.tags = item.tags.filter(t => t.id != tagId);
      this.getUniqeTags(this.selectedList);
    }, error => alert(error));
  }
  //Filter the tag
  filterTag(): void {
    this.itemsClient.filterTodoItemsByTag(this.selectedTag, this.selectedList.id).subscribe(result => {
      this.selectedList.items = result;
    }, error => alert(error));
  }
  //End of My Methods
  updateItemDetails(): void {
    const item = new UpdateTodoItemDetailCommand(this.itemDetailsFormGroup.value);
    this.itemsClient.updateItemDetails(this.selectedItem.id, item).subscribe(
      () => {
        if (this.selectedItem.listId !== item.listId) {
          this.selectedList.items = this.selectedList.items.filter(
            i => i.id !== this.selectedItem.id
          );
          const listIndex = this.lists.findIndex(
            l => l.id === item.listId
          );
          this.selectedItem.listId = item.listId;
          this.lists[listIndex].items.push(this.selectedItem);
        }

        this.selectedItem.priority = item.priority;
        this.selectedItem.note = item.note;
        this.itemDetailsModalRef.hide();
        this.itemDetailsFormGroup.reset();
      },
      error => console.error(error)
    );
  }

  addItem() {
    const item = {
      id: 0,
      listId: this.selectedList.id,
      priority: this.priorityLevels[0].value,
      title: '',
      done: false
    } as TodoItemDto;

    this.selectedList.items.push(item);
    const index = this.selectedList.items.length - 1;
    this.editItem(item, 'itemTitle' + index);
  }

  editItem(item: TodoItemDto, inputId: string): void {
    this.selectedItem = item;
    setTimeout(() => document.getElementById(inputId).focus(), 100);
  }

  updateItem(item: TodoItemDto, pressedEnter: boolean = false): void {
    const isNewItem = item.id === 0;

    if (!item.title.trim()) {
      this.deleteItem(item);
      return;
    }

    if (item.id === 0) {
      this.itemsClient
        .create({
          ...item, listId: this.selectedList.id
        } as CreateTodoItemCommand)
        .subscribe(
          result => {
            item.id = result;
          },
          error => console.error(error)
        );
    } else {
      this.itemsClient.update(item.id, item).subscribe(
        () => console.log('Update succeeded.'),
        error => console.error(error)
      );
    }

    this.selectedItem = null;

    if (isNewItem && pressedEnter) {
      setTimeout(() => this.addItem(), 250);
    }
  }

  deleteItem(item: TodoItemDto, countDown?: boolean) {
    if (countDown) {
      if (this.deleting) {
        this.stopDeleteCountDown();
        return;
      }
      this.deleteCountDown = 3;
      this.deleting = true;
      this.deleteCountDownInterval = setInterval(() => {
        if (this.deleting && --this.deleteCountDown <= 0) {
          this.deleteItem(item, false);
        }
      }, 1000);
      return;
    }
    this.deleting = false;
    if (this.itemDetailsModalRef) {
      this.itemDetailsModalRef.hide();
    }

    if (item.id === 0) {
      const itemIndex = this.selectedList.items.indexOf(this.selectedItem);
      this.selectedList.items.splice(itemIndex, 1);
    } else {
      this.itemsClient.delete(item.id).subscribe(
        () => {

          this.getUniqeTags(this.selectedList);
          (this.selectedList.items = this.selectedList.items.filter(
            t => t.id !== item.id
          ))
        },
        error => console.error(error)
      );
    }
  }

  stopDeleteCountDown() {
    clearInterval(this.deleteCountDownInterval);
    this.deleteCountDown = 0;
    this.deleting = false;
  }
}
