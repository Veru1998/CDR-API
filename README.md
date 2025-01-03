# CDR-API

CDR API provides analytics endpoints to get information about Call Detail Records saved in DB.

**Used technologies:**

1.  .NET Web API Project = CDR
2.  xUnit Tests Project = CDR.Tests
3.  SQL DB
4.  Swagger for endpoints documentation. [localhost link](http://localhost:5243/swagger/index.html)

I decided to implement the solution as Web API because I think it provides a robust framework for building RESTful services, which is ideal for building scalable, maintainable and efficient APIs. It integrates well with SQL databases and offers excellent support for asynchronous processing, which is crucial when handling large CDR files. I chose SQL DB because it is well suited to handle structured data, which are typically stored in tabular form, like CDR records. For testing I chose xUnit because it is widely used and well-supported testing framework.

### Future enhancements and considerations

- Create hosted SQL DB and replace with local instance.
- We could implement also custom error response handling if needed in future.
- For endpoints which return lists we could also add pagination to return the data in bundles.

#### New endpoints

- **top n callers** - most frequent callers from all records or by date range
- **top n recipients** - most frequent recipients from all records or by date range
- **cheapest/most expensive call** - chosen by callerId or time range
- **shortest/longest call** - chosen by callerId or time range

#### Improvements for existing endpoints

- `/call-volume` and `/total-calls` are the same just differ in selecting by date range or callerId and thus can be merged together
- We can also extend filtering from where the data will be taken for some endpoints (for example in some we are missing selecting from all records - `/call-volume` or `/summary`).
