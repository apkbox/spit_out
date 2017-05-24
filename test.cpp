#include <memory>
#include <cinttypes>
#include <string>

class FileReadStream {
public:
  explicit FileReadStream(const std::string &name) : handle_(0) {}
  FileReadStream(FileReadStream &&other) : handle_(other.handle_) {
    other.handle_ = -1;
  }

  FileReadStream &operator=(FileReadStream &&other) {
    handle_ = other.handle_;
    other.handle_ = -1;
    return *this;
  }

  FileReadStream(const FileReadStream &other) = delete;
  FileReadStream &operator=(const FileReadStream &other) = delete;

  ~FileReadStream() { /* close(handle_); */
  }

  size_t Read(uint8_t *buffer, size_t length) { return 0; }

private:
  int handle_;
};

class Consumer {
public:
  // Consumer takes ownership
  explicit Consumer(std::unique_ptr<FileReadStream> stream)
      : stream_(std::move(stream)) {}

  // Consumer shares ownership
  explicit Consumer(const std::shared_ptr<FileReadStream> &stream)
      : stream_(stream) {}

  // Consumer shares ownership of locally allocated stream
  explicit Consumer(const FileReadStream &stream)
      : stream_(&stream, [](FileReadStream *) {}) {}

private:
  std::shared_ptr<FileReadStream> stream_;
};

int main() {
  Consumer consumer1{ std::make_unique<FileReadStream>("test") };

  std::shared_ptr<FileReadStream> shared_stream{ "test" };
  Consumer consumer2{ shared_stream };

  FileReadStream local_stream{ "test" };
  Consumer consumer3(local_stream);

  return 0;
}